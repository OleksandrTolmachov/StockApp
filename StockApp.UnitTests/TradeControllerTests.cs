namespace Tests.UnitTests;

public class TradeControllerTests
{
    private readonly IFinnHubService _finnHubService;
    private readonly IStockService _stockService;
    private readonly IOptionsMonitor<StockOptions> _stockOptions;
    private readonly IMapper _mapper;

    private readonly Mock<IFinnHubService> _mockFinnHubService;
    private readonly Mock<IStockService> _mockStockService;
    private readonly Mock<IOptionsMonitor<StockOptions>> _mockStockOptions;
    private readonly Mock<IMapper> _mockMapper;

    private readonly IFixture _fixture;

    private readonly TradeController _controller;

    public TradeControllerTests()
    {
        _fixture = new Fixture();

        _mockFinnHubService = new();
        _mockStockService = new();
        _mockMapper = new();
        _mockStockOptions = new();

        _finnHubService = _mockFinnHubService.Object;
        _stockService = _mockStockService.Object;
        _mapper = _mockMapper.Object;
        _stockOptions = _mockStockOptions.Object;

        _controller = new TradeController(_finnHubService,
            _stockService, _stockOptions, _mapper);
    }

    [Fact]
    public async Task TradeStock_ReturnsViewWithStock_ValidRequest()
    {
        _mockStockOptions.Setup(opt => opt.CurrentValue)
            .Returns(_fixture.Create<StockOptions>());

        _mockFinnHubService.Setup(service => service.GetStockPriceQuote(It.IsAny<string>()))
            .ReturnsAsync(new Dictionary<string, object> { { "c", 100 } })
            .Verifiable();

        _mockFinnHubService.Setup(service => service.GetCompanyProfile(It.IsAny<string>()))
            .ReturnsAsync(new Dictionary<string, object> { { "ticker", "MSFT" }, { "name", "Microsoft" } })
            .Verifiable();

        var request = _fixture.Create<OrderRequest>();

        var result = await _controller.TradeStock(request);

        var stockView = Assert.IsAssignableFrom<ViewResult>(result);
        var model = stockView.ViewData.Model;

        var stockTrade = Assert.IsAssignableFrom<StockTrade>(model);
        stockTrade.StockSymbol.Should().NotBeNullOrEmpty();
        stockTrade.Price.Should().NotBeNull();
        _mockFinnHubService.Verify();
    }

    [Fact]
    public async Task BuyOrder_ReturnsRedirectToAction_ValidModel()
    {
        _mockStockService.Setup(service => service.CreateBuyOrderAsync(It.IsAny<OrderRequest>()))
            .Verifiable();
        var result = await _controller.BuyOrder(_fixture.Create<OrderRequest>());

        var redirect = Assert.IsAssignableFrom<RedirectToActionResult>(result);
        redirect.ActionName.Should().Be("Orders");
        _mockStockService.Verify();
    }

    [Fact]
    public async Task Orders_ReturnViewResult()
    {
        _mockStockService.Setup(service => service.GetBuyOrdersAsync())
            .ReturnsAsync(new List<OrderResponse>());
        _mockStockService.Setup(service => service.GetSellOrdersAsync())
            .ReturnsAsync(new List<OrderResponse>());

        var result = await _controller.Orders();

        var viewResult = Assert.IsAssignableFrom<ViewResult>(result);
        viewResult.Model.Should().BeAssignableTo<Orders>();
    }
}
