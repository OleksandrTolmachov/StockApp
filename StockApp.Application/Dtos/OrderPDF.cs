using System.ComponentModel.DataAnnotations;
namespace StockApp.Application.DTO;
public enum OrdersType
{
    Buy,
    Sell
}

public class OrderPDF
{
    public string? Stock { get; set; }

    [Display(Name = "Date and time")]
    public DateTime? DateTimeOffer { get; set; }
    public OrdersType OrderType { get; set; }
    public uint Quantity { get; set; }
    public double Price { get; set; }

    [Display(Name = "Trade amount")]
    public double TradeAmount => Math.Round(Price * Quantity, 2);
}
