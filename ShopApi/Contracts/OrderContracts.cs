namespace ShopApi.Contracts;

public record CreateOrderRequest(string CustomerEmail, List<CreateOrderLineRequest> Lines);
public record CreateOrderLineRequest(Guid ProductId, int Quantity);

public record OrderResponse(Guid Id, string CustomerEmail, DateTime CreatedAtUtc, decimal Total, List<OrderLineResponse> Lines);
public record OrderLineResponse(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);
