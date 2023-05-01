using Microsoft.Extensions.Options;
using StockApp.Application.Options;
using StockApp.Application.RepositoryContracts;

namespace StockApp.Application.Services;

public class FinnHubService : IFinnHubService
{
    private readonly IFinnhubRepository _finnhubRepository;
    private readonly IOptionsMonitor<StockOptions> _stockOptions;

    public FinnHubService(IFinnhubRepository finnhubRepository,
        IOptionsMonitor<StockOptions> optionsMonitor)
    {
        _finnhubRepository = finnhubRepository;
        _stockOptions = optionsMonitor;
    }


    public async Task<Dictionary<string, object>?> GetCompanyProfile(string? stockSymbol)
    {
        if (stockSymbol is null) return default;
        return await _finnhubRepository.GetCompanyProfileAsync(stockSymbol);
    }

    public async Task<Dictionary<string, object>?> GetStockPriceQuote(string? stockSymbol)
    {
        if (stockSymbol is null) return default;
        return await _finnhubRepository.GetStockPriceQuoteAsync(stockSymbol);
    }

    public async Task<Dictionary<string, object>?> GetStock(string? stockSymbolToSearch)
    {
        if (stockSymbolToSearch is null) return default;
        return await _finnhubRepository.SearchStocksAsync(stockSymbolToSearch);
    }

    public async Task<List<Dictionary<string, string>>?> GetStocks()
    {
        return await _finnhubRepository.GetStocksAsync();
    }

    public async Task<List<Dictionary<string, string>>?> GetFilteredStocks(bool topOnly)
    {
        var stocks = await _finnhubRepository.GetStocksAsync();
        if (!topOnly || stocks is null) return stocks;

        var topStocks = _stockOptions.CurrentValue.TopStocks;
        return stocks.Where(s => topStocks?.Contains(s["symbol"]) ?? true).ToList();
    }
}
