using StockApp.Application.DTO;
using System.Collections.ObjectModel;

namespace StockApp.WebUI.ViewModels;

public class Orders
{
    private readonly List<OrderResponse> buyOrders;
    private readonly List<OrderResponse> sellOrders;

    public Orders(List<OrderResponse> buyOrders, List<OrderResponse> sellOrders)
    {
        this.buyOrders = new(buyOrders);
        this.sellOrders = new(sellOrders);
    }

    public ReadOnlyCollection<OrderResponse> BuyOrders => buyOrders.AsReadOnly();
    public ReadOnlyCollection<OrderResponse> SellOrders => sellOrders.AsReadOnly();
}
