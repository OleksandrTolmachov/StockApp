namespace StockApp.Application.DTO;

public interface IOrderRequest
{
    uint Quantity { get; set; }
    string? StockSymbol { get; set; }
}
