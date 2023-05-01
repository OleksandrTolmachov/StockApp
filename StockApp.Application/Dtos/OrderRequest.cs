using System.ComponentModel.DataAnnotations;

namespace StockApp.Application.DTO;

public class OrderRequest : IOrderRequest
{
    [Range(1, 10_000)]

    public uint Quantity { get; set; }

    [Required]
    public string? StockSymbol { get; set; }
}
