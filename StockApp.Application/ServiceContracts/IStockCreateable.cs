using StockApp.Application.DTO;

namespace StockApp.Application.Services;

public interface IStockCreateable
{
    public Task CreateBuyOrderAsync(IOrderRequest buyOrderRequest);

    public Task CreateSellOrderAsync(IOrderRequest sellOrderRequest);
}
