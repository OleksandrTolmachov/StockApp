using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using StockApp.Application.DTO;
using StockApp.Application.Options;
using StockApp.Application.Services;
using StockApp.WebUI.Filters.ActionFilters;
using StockApp.WebUI.ViewModels;

namespace StockApp.WebUI.Controllers;

[Route("{controller}/{action}")]
public class TradeController : Controller
{
    private readonly IFinnHubService _finnHubService;
    private readonly IStockService _stockService;
    private readonly IOptionsMonitor<StockOptions> _stockOptions;
    private readonly IMapper _mapper;

    public TradeController(IFinnHubService finnHubService,
        IStockService stockService,
        IOptionsMonitor<StockOptions> stockOptions,
        IMapper mapper)
    {
        _finnHubService = finnHubService;
        _stockOptions = stockOptions;
        _mapper = mapper;
        _stockService = stockService;
    }

    public async Task<IActionResult> TradeStock(OrderRequest orderRequest)
    {
        var options = _stockOptions.CurrentValue;
        if(!ModelState.IsValid) {
            ViewBag.Error = "Invalid input.";
            orderRequest.Quantity = options.DefaultOrderQuantity;
        }
        var stockTrade = await GetStockTrade(orderRequest.StockSymbol ?? options.DefaultStockSymbol,
            orderRequest.Quantity);

        if (stockTrade is null) return BadRequest(orderRequest.StockSymbol);

        return View(stockTrade);
    }

    [HttpPost]
    [ValidationErrorFilterFactory("TradeStock")]
    public async Task<IActionResult> BuyOrder([FromForm] OrderRequest orderRequest)
    {
        await _stockService.CreateBuyOrderAsync(orderRequest);
        return RedirectToAction("Orders");
    }

    [HttpPost]
    [ValidationErrorFilterFactory("TradeStock")]
    public async Task<IActionResult> SellOrder([FromForm] OrderRequest orderRequest)
    {
        await _stockService.CreateSellOrderAsync(orderRequest);
        return RedirectToAction("Orders");
    }

    public async Task<IActionResult> Orders()
    {
        var orders = new Orders(await _stockService.GetBuyOrdersAsync(), await _stockService.GetSellOrdersAsync());
        return View(orders);
    }

    public async Task<IActionResult> OrdersPDF()
    {
        var sellOrders = await _stockService.GetSellOrdersAsync();
        var sellOrdersView = _mapper.Map<List<OrderPDF>>(sellOrders);

        var buyOrders = await _stockService.GetBuyOrdersAsync();
        var buyOrdersView = _mapper.Map<List<OrderPDF>>(buyOrders);

        return new ViewAsPdf("OrdersPDF", sellOrdersView.Union(buyOrdersView))
        { PageMargins = new(20, 20, 20, 20) };
    }

    [NonAction]
    private async Task<StockTrade?> GetStockTrade(string? symbol, uint defaultQuantity)
    {
        var stockPriceResponse = await _finnHubService.GetStockPriceQuote(symbol);
        var profileResponse = await _finnHubService.GetCompanyProfile(symbol);
        if (stockPriceResponse is null || profileResponse is null || profileResponse?.Count == 0)
            return null;

        return new StockTrade()
        {
            Price = double.Parse(stockPriceResponse["c"]!.ToString()!).ToString("c"),
            StockSymbol = profileResponse!["ticker"].ToString(),
            StockName = profileResponse["name"].ToString(),
            Quantity = defaultQuantity
        };
    }
}
