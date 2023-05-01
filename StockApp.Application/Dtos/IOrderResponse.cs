namespace StockApp.Application.DTO;

public interface IOrderResponse
{
    DateTime? DateTimeOffer { get; set; }
    Guid Id { get; set; }
    double Price { get; set; }
    uint Quantity { get; set; }
    string Stock { get; }
    string? StockName { get; set; }
    string? StockSymbol { get; set; }
}