using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ShopApi.Contracts;
using ShopApi.Data;
using ShopApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShopDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=shop.db"));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient<ValidationExceptionMiddleware>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ValidationExceptionMiddleware>();

// Health-ish
app.MapGet("/", () => Results.Ok(new { status = "ok" }));

// POST /products
app.MapPost("/products", async (CreateProductRequest req, IValidator<CreateProductRequest> validator, ShopDbContext db) =>
{
    var result = await validator.ValidateAsync(req);
    if (!result.IsValid) throw new ValidationException(result.Errors);

    var product = new Product { Id = Guid.NewGuid(), Name = req.Name.Trim(), Price = req.Price };
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/products/{product.Id}", new ProductResponse(product.Id, product.Name, product.Price));
});

// GET /products/{id}
app.MapGet("/products/{id:guid}", async (Guid id, ShopDbContext db) =>
{
    var p = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    return p is null
        ? Results.NotFound()
        : Results.Ok(new ProductResponse(p.Id, p.Name, p.Price));
});

// POST /orders (flujo completo + errores)
app.MapPost("/orders", async (CreateOrderRequest req, IValidator<CreateOrderRequest> validator, ShopDbContext db) =>
{
    var v = await validator.ValidateAsync(req);
    if (!v.IsValid) throw new ValidationException(v.Errors);

    // Comprobar que todos los productos existen
    var productIds = req.Lines.Select(x => x.ProductId).Distinct().ToList();
    var products = await db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

    if (products.Count != productIds.Count)
        return Results.NotFound(new { error = "One or more products were not found" });

    var order = new Order
    {
        Id = Guid.NewGuid(),
        CustomerEmail = req.CustomerEmail.Trim(),
        CreatedAtUtc = DateTime.UtcNow,
        Lines = req.Lines.Select(l => new OrderLine
        {
            Id = Guid.NewGuid(),
            ProductId = l.ProductId,
            Quantity = l.Quantity
        }).ToList()
    };

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    return Results.Created($"/orders/{order.Id}", new { order.Id });
});

// GET /orders/{id} (para comprobar totales)
app.MapGet("/orders/{id:guid}", async (Guid id, ShopDbContext db) =>
{
    var order = await db.Orders
        .AsNoTracking()
        .Include(o => o.Lines)
        .ThenInclude(l => l.Product)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (order is null) return Results.NotFound();

    var lines = order.Lines.Select(l =>
    {
        var lineTotal = l.Product.Price * l.Quantity;
        return new OrderLineResponse(l.ProductId, l.Product.Name, l.Product.Price, l.Quantity, lineTotal);
    }).ToList();

    var total = lines.Sum(x => x.LineTotal);

    return Results.Ok(new OrderResponse(order.Id, order.CustomerEmail, order.CreatedAtUtc, total, lines));
});

app.Run();

public partial class Program { } //necesario para WebApplicationFactory
