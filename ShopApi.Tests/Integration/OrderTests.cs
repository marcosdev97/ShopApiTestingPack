using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShopApi.Contracts;
using ShopApi.Tests.Infrastructure;
using Xunit;

namespace ShopApi.Tests.Integration;

public class OrdersTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrdersTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Full_flow_create_product_then_create_order_then_get_order()
    {
        // 1) Create product
        var createProduct = new CreateProductRequest("Mouse", 25m);
        var productRes = await _client.PostAsJsonAsync("/products", createProduct);
        productRes.EnsureSuccessStatusCode();

        var product = await productRes.Content.ReadFromJsonAsync<ProductResponse>();
        product.Should().NotBeNull();

        // 2) Create order with that product
        var orderReq = new CreateOrderRequest(
            "buyer@test.com",
            new List<CreateOrderLineRequest> { new(product!.Id, 2) }
        );

        var orderRes = await _client.PostAsJsonAsync("/orders", orderReq);
        orderRes.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await orderRes.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        created.Should().NotBeNull();
        var orderId = Guid.Parse(created!["id"]);

        // 3) Get order and validate totals
        var getRes = await _client.GetAsync($"/orders/{orderId}");
        getRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await getRes.Content.ReadFromJsonAsync<OrderResponse>();
        order.Should().NotBeNull();
        order!.Total.Should().Be(50m); // 25 * 2
        order.Lines.Should().HaveCount(1);
        order.Lines[0].LineTotal.Should().Be(50m);
    }

    [Fact]
    public async Task Post_orders_should_return_404_when_product_missing()
    {
        var orderReq = new CreateOrderRequest(
            "buyer@test.com",
            new List<CreateOrderLineRequest> { new(Guid.NewGuid(), 1) }
        );

        var res = await _client.PostAsJsonAsync("/orders", orderReq);
        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
