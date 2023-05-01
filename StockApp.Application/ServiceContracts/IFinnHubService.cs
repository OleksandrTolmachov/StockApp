namespace StockApp.Application.Services;

public interface IFinnHubService
{
    public Task<Dictionary<string, object>?> GetCompanyProfile(string? stockSymbol);

    public Task<Dictionary<string, object>?> GetStockPriceQuote(string? stockSymbol);

    public Task<Dictionary<string, object>?> GetStock(string? stockSymbolToSearch);

    public Task<List<Dictionary<string, string>>?> GetStocks();

    public Task<List<Dictionary<string, string>>?> GetFilteredStocks(bool topOnly);
}