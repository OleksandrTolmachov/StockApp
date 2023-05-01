using Domain.Models;
using Microsoft.EntityFrameworkCore;
using StockApp.Application.RepositoryContracts;

namespace StockApp.Infrastructure.Repositories;

public class StocksRepository : IStocksRepository
{
    private readonly OrdersDbContext _dbContext;

    public StocksRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BuyOrder> CreateBuyOrderAsync(BuyOrder buyOrder)
    {
        await _dbContext.BuyOrders.AddAsync(buyOrder);
        await _dbContext.SaveChangesAsync();
        return buyOrder;
    }

    public async Task<SellOrder> CreateSellOrderAsync(SellOrder sellOrder)
    {
        await _dbContext.SellOrders.AddAsync(sellOrder);
        await _dbContext.SaveChangesAsync();
        return sellOrder;
    }

    public async Task<List<BuyOrder>> GetBuyOrdersAsync()
    {
        return await _dbContext.BuyOrders.ToListAsync();
    }

    public async Task<List<SellOrder>> GetSellOrdersAsync()
    {
        return await _dbContext.SellOrders.ToListAsync();
    }
}
