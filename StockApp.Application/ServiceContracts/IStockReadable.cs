using StockApp.Application.DTO;

namespace StockApp.Application.Services;

public interface IStockReadable
{
    public Task<List<OrderResponse>> GetBuyOrdersAsync();

    public Task<List<OrderResponse>> GetSellOrdersAsync();
}
