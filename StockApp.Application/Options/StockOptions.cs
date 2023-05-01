using System.ComponentModel.DataAnnotations;

namespace StockApp.Application.Options;

public class StockOptions
{
    [Required]
    public string? DefaultStockSymbol { get; set; }

    [Required]
    public string[]? TopStocks { get; set; }

    [Required]
    public uint DefaultOrderQuantity { get; set; }
}
