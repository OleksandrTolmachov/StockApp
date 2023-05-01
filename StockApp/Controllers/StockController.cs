using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Services;

namespace StockApp.WebUI.Controllers;

[Route("{controller=Stock}/{action=Explore}")]
public class StockController : Controller
{
    private readonly IFinnHubService _finnHubService;

    public StockController(IFinnHubService finnHubService)
    {
        _finnHubService = finnHubService;
    }

    [HttpGet]
    public async Task<IActionResult> Explore()
    {
        var stocks = await _finnHubService.GetFilteredStocks(topOnly: true);
        return View(stocks);
    }

    [HttpGet("{stockSymbol}")]
    public async Task<ActionResult> GetStockCard(string? stockSymbol)
    {
        var stockPriceResponse = await _finnHubService.GetStockPriceQuote(stockSymbol);
        var profileResponse = await _finnHubService.GetCompanyProfile(stockSymbol);
        if (stockPriceResponse is null || profileResponse?.Count is 0 or null)
            return BadRequest();

        return PartialView("_StockCard", stockPriceResponse.Union(profileResponse)
            .ToDictionary(keyValue => keyValue.Key, keyValue => keyValue.Value));
    }
}
