using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockApp.Application.Options;
using StockApp.Application.Services;

namespace StockApp.WebUI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class StockApiController : ControllerBase
{
    private readonly IFinnHubService _finnHubService;
    private readonly IOptionsMonitor<StockOptions> _stockOptions;

    public StockApiController(IFinnHubService finnHubService, IOptionsMonitor<StockOptions> stockOptions)
    {
        _finnHubService = finnHubService;
        _stockOptions = stockOptions;
    }

    public async Task<ActionResult> GetPrice()
    {
        var response = await _finnHubService.GetStockPriceQuote
            (_stockOptions.CurrentValue.DefaultStockSymbol);
        if (response is not null)
        {
            Response.Headers.AccessControlAllowOrigin = "*";
            return new JsonResult(new { price = response["c"] });
        }
        return BadRequest(response);
    }
}

