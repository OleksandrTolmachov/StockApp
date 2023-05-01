namespace StockApp.Application.DTO;

public record class OrderResponse : IOrderResponse
{
    public Guid Id { get; set; }
    public string? StockSymbol { get; set; }
    public string? StockName { get; set; }
    public string Stock => $"{StockName}({StockSymbol})";
    public DateTime? DateTimeOffer { get; set; }
    public uint Quantity { get; set; }
    public double Price { get; set; }
    public double TradeAmount => Price * Quantity;
}
