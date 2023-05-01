using AutoMapper;
using Domain.Models;
using Services.Helpers;
using StockApp.Application.DTO;
using StockApp.Application.RepositoryContracts;

namespace StockApp.Application.Services;

public class StockService : IStockService
{
    private readonly IFinnHubService _finnHubService;
    private readonly IStocksRepository _stocksRepository;
    private readonly IMapper _mapper;

    public StockService(IFinnHubService finnHubService, IStocksRepository stocksRepository, IMapper mapper)
    {
        _finnHubService = finnHubService;
        _stocksRepository = stocksRepository;
        _mapper = mapper;
    }

    public async Task CreateBuyOrderAsync(IOrderRequest buyOrderRequest)
    {
        if (buyOrderRequest is null || buyOrderRequest.StockSymbol is null)
            throw new ArgumentNullException(nameof(buyOrderRequest));
        ValidationHelper.ValidateModel(buyOrderRequest);

        var buyOrder = _mapper.Map<BuyOrder>(buyOrderRequest);
        await FormOrder(buyOrder, buyOrderRequest.StockSymbol);

        await _stocksRepository.CreateBuyOrderAsync(buyOrder);
    }

    public async Task CreateSellOrderAsync(IOrderRequest sellOrderRequest)
    {
        if (sellOrderRequest is null || sellOrderRequest.StockSymbol is null)
            throw new ArgumentNullException(nameof(sellOrderRequest));
        ValidationHelper.ValidateModel(sellOrderRequest);

        var sellOrder = _mapper.Map<SellOrder>(sellOrderRequest);
        await FormOrder(sellOrder, sellOrderRequest.StockSymbol);

        await _stocksRepository.CreateSellOrderAsync(sellOrder);
    }

    public async Task<List<OrderResponse>> GetBuyOrdersAsync()
    {
        return (await _stocksRepository.GetBuyOrdersAsync())
            .Select(buyOrder => _mapper.Map<OrderResponse>(buyOrder))
            .ToList();
    }

    public async Task<List<OrderResponse>> GetSellOrdersAsync()
    {
        return (await _stocksRepository.GetSellOrdersAsync())
            .Select(sellOrder => _mapper.Map<OrderResponse>(sellOrder))
            .ToList();
    }

    private async Task<double> GetPrice(string? stockSymbol)
    {
        if (stockSymbol is null) return 0;
        var response = await _finnHubService.GetStockPriceQuote(stockSymbol);
        if (response is null) return 0;
        double price = Convert.ToDouble(response["c"].ToString());

        return price;
    }

    private async Task<string> GetName(string? stockSymbol)
    {
        if (stockSymbol is null) return "";
        var response = await _finnHubService.GetCompanyProfile(stockSymbol);
        if (response is null) return "";
        string name = response["name"]?.ToString() ?? "";

        return name;
    }

    private async Task FormOrder(IOrder order, string stockSymbol)
    {
        order.Id = Guid.NewGuid();
        order.DateTimeOffer = DateTime.Now;

        double price = await GetPrice(stockSymbol);
        if (price == 0) throw new ArgumentException($"{nameof(stockSymbol)} is not valid symbol");
        order.Price = price;

        order.StockName = await GetName(stockSymbol);
    }
}