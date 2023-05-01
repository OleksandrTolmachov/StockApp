using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StockApp.Application.DTO;

namespace StockApp.WebUI.Filters.ActionFilters;

[AttributeUsage(AttributeTargets.Method)]
public class ValidationErrorFilterFactory : Attribute, IFilterFactory
{
    public bool IsReusable => false;

    private readonly string _action;

    public ValidationErrorFilterFactory(string action)
    {
        _action = action;
    }

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return new ValidationErrorActionFilter(_action);
    }
}

public class ValidationErrorActionFilter : IAsyncActionFilter
{
    private readonly string _action;

    public ValidationErrorActionFilter(string action)
    {
        _action = action;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var controller = (Controller)context.Controller;

            var orderRequest = context.ActionArguments["orderRequest"] as IOrderRequest;
            object? routeValues = null;
            if (orderRequest is not null)
                routeValues = new { orderRequest.StockSymbol, orderRequest.Quantity };

            //context.Result = controller.View("TradeStock", orderRequest);
            context.Result = controller.RedirectToAction(_action, routeValues);
            return;
        }

        await next();
    }
}
