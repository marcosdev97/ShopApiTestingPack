using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShopApi.Contracts;
using ShopApi.Tests.Infrastructure;
using Xunit;

namespace ShopApi.Tests.Integration;

public class ProductsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_products_should_create_product()
    {
        var req = new CreateProductRequest("Keyboard", 49.99m);

        var res = await _client.PostAsJsonAsync("/products", req);

        res.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await res.Content.ReadFromJsonAsync<ProductResponse>();
        body.Should().NotBeNull();
        body!.Name.Should().Be("Keyboard");
        body.Price.Should().Be(49.99m);
        body.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Post_products_should_return_400_when_invalid()
    {
        var req = new CreateProductRequest("", 0);

        var res = await _client.PostAsJsonAsync("/products", req);

        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await res.Content.ReadAsStringAsync();
        json.Should().Contain("errors");
        json.Should().Contain("Name");
        json.Should().Contain("Price");
    }

    [Fact]
    public async Task Get_product_should_return_404_when_not_found()
    {
        var res = await _client.GetAsync($"/products/{Guid.NewGuid()}");
        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
