using AutoFixture;
using FluentAssertions;
using StockApp.Application.DTO;
using Tests.Helpers;

namespace Tests.IntegrationTests;

public class TradeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory _factory;
    private readonly IFixture _fixture;

    public TradeControllerIntegrationTests(CustomWebApplicationFactory webApplicationFactory)
    {
        _fixture = new Fixture();
        _factory = webApplicationFactory;
        _httpClient = webApplicationFactory.CreateDefaultClient();
    }

    [Fact]
    public async Task Get_GetStock_ReturnsSuccess()
    {
        string stockSymbol = "MSFT";
        var response = await _httpClient.GetAsync($"Trade/TradeStock?stocksymbol={stockSymbol}");

        var document = await HtmlHelper.GetDocumentAsync(response);

        response.Should().BeSuccessful();
        response.Content.Headers.ContentType!.ToString()
            .Should().Be("text/html; charset=utf-8");

        document.QuerySelector("#stock-symbol")!.TextContent.Should().Be(stockSymbol);
        document.QuerySelector("#stock-name")!.TextContent.Should().NotBeNullOrEmpty();
        document.QuerySelector("#main-price")!.TextContent.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Get_GetStock_ReturnsBadRequest()
    {
        string stockSymbol = _fixture.Create<string>();
        var response = await _httpClient.GetAsync($"Trade/TradeStock?stocksymbol={stockSymbol}");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_FailedBuyOrder_ReturnsRedirectToAction()
    {
        var buyOrderRequest = _fixture.Build<OrderRequest>()
            .With(bor => bor.StockSymbol, null as string)
            .Create();

        var response = await _httpClient.PostAsFormUrlEncodedAsync("/Trade/BuyOrder", buyOrderRequest);

        response.Should().BeRedirection();

        response.Headers.Location!.OriginalString.Split("?")[0]
            .Should().Be("/Trade/TradeStock");
    }

    [Fact]
    public async Task Post_SuccessBuyOrder_ReturnsRedirectToAction()
    {
        string stockSymbol = "MSFT";
        uint quantity = 1;
        var buyOrderRequest = new OrderRequest() { Quantity = quantity, StockSymbol = stockSymbol };

        var response = await _httpClient.PostAsFormUrlEncodedAsync("/Trade/BuyOrder", buyOrderRequest);
        var ordersResponse = await _httpClient.GetAsync("/Trade/Orders");
        var document = await HtmlHelper.GetDocumentAsync(ordersResponse);

        response.Should().BeRedirection();
        response.Headers.Location.Should().Be("/Trade/Orders");

        var buyItem = document.QuerySelectorAll(".buytrade-item").Single(el =>
            el.QuerySelector(".stock-buysymbol")!.TextContent == stockSymbol
            && el.QuerySelector(".buytrade-quantity")!.TextContent == quantity.ToString()
            && DateTime.Parse(el.QuerySelector(".buytime")!.TextContent).Hour == DateTime.Now.Hour);

        buyItem.Should().NotBeNull();
    }
}
