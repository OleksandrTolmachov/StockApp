using Microsoft.AspNetCore.Mvc;

namespace StockApp.WebUI.Controllers;

public class GeneralController : Controller
{
    [Route("/Error")]
    public IActionResult Error()
    {
        ViewBag.StatusCode = Response.StatusCode;
        ViewBag.RequestId = HttpContext.TraceIdentifier;
        return View();
    }
}
