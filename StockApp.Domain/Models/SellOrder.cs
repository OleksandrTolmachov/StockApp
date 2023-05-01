using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public record class SellOrder : IOrder
{
    [Key]
    public Guid Id { get; set; }
    public string? StockSymbol { get; set; }
    public string? StockName { get; set; }
    public DateTime? DateTimeOffer { get; set; }
    public uint Quantity { get; set; }
    public double Price { get; set; }
}