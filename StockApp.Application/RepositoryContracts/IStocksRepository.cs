using Domain.Models;

namespace StockApp.Application.RepositoryContracts;

public interface IStocksRepository
{
    public Task<BuyOrder> CreateBuyOrderAsync(BuyOrder buyOrder);

    public Task<SellOrder> CreateSellOrderAsync(SellOrder sellOrder);
    public Task<List<BuyOrder>> GetBuyOrdersAsync();

    public Task<List<SellOrder>> GetSellOrdersAsync();
}
