using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockApp.Application.Options;
using StockApp.Application.RepositoryContracts;
using System.Net.Http.Json;

namespace StockApp.Infrastructure.Repositories;

public class FinnhubRepository : IFinnhubRepository
{
    private const string _clientName = "Finnhub";

    private readonly HttpClient _httpClient;
    private readonly IOptionsMonitor<TokenOption> _tokenOption;
    private readonly ILogger<FinnhubRepository> _logger;

    public FinnhubRepository(IHttpClientFactory httpClientFactory,
        IOptionsMonitor<TokenOption> tokenOption,
        ILogger<FinnhubRepository> logger)
    {
        _tokenOption = tokenOption;
        _httpClient = httpClientFactory.CreateClient(_clientName);
        _logger = logger;
    }

    public async Task<Dictionary<string, object>?> GetCompanyProfileAsync(string stockSymbol)
    {
        return await GetResponseFromApiAsync<Dictionary<string, object>?>
            ($"stock/profile2?symbol={stockSymbol}&token={_tokenOption.CurrentValue.Token}");
    }

    public async Task<Dictionary<string, object>?> GetStockPriceQuoteAsync(string stockSymbol)
    {
        return await GetResponseFromApiAsync<Dictionary<string, object>?>
            ($"quote?symbol={stockSymbol}&token={_tokenOption.CurrentValue.Token}");
    }

    public async Task<List<Dictionary<string, string>>?> GetStocksAsync()
    {
        return await GetResponseFromApiAsync<List<Dictionary<string, string>>?>
            ($"stock/symbol?exchange=US&token={_tokenOption.CurrentValue.Token}");
    }

    public async Task<Dictionary<string, object>?> SearchStocksAsync(string stockSymbolToSearch)
    {
        return await GetResponseFromApiAsync<Dictionary<string, object>?>
            ($"stock/profile2?symbol={stockSymbolToSearch}" +
            $"&token={_tokenOption.CurrentValue.Token}");
    }

    private static readonly SemaphoreSlim _semaphoreSlim = new(1);
    private async Task<T?> GetResponseFromApiAsync<T>(string url)
    {
        try
        {
            await _semaphoreSlim.WaitAsync();

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return default;

            var responseBody = await response.Content.ReadFromJsonAsync<T>();
            return responseBody;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Class}.{Method}\n{ExceptionType}\n{Message}",
                nameof(FinnhubRepository), nameof(GetResponseFromApiAsync),
                ex.GetType().ToString(), ex.Message);

            throw;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}