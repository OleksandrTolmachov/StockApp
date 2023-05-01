namespace Domain.Models;

public interface IOrder
{
    DateTime? DateTimeOffer { get; set; }
    Guid Id { get; set; }
    double Price { get; set; }
    uint Quantity { get; set; }
    string? StockName { get; set; }
    string? StockSymbol { get; set; }
}
