using Domain.Models;
using StockApp.Application.RepositoryContracts;

namespace StocksServiceTests
{
    public class StockServiceTest
    {
        private readonly StockService stockService;
        private readonly Fixture _fixture;
        private readonly Mock<IFinnHubService> _mockFinnHubService;
        private readonly IFinnHubService _finnHubService;
        private readonly Mock<IStocksRepository> _mockStockRepository;
        private readonly IStocksRepository _stocksRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IMapper _mapper;

        public StockServiceTest()
        {
            _fixture = new Fixture();

            _mockFinnHubService = new Mock<IFinnHubService>();
            _finnHubService = _mockFinnHubService.Object;

            _mockStockRepository = new Mock<IStocksRepository>();
            _stocksRepository = _mockStockRepository.Object;

            _mockMapper = new Mock<IMapper>();
            _mapper = _mockMapper.Object;

            stockService = new StockService(_finnHubService, _stocksRepository, _mapper);
        }

        #region CreateBuyOrder
        [Fact]
        public async Task CreateBuyOrder_BuyOrderIsNull()
        {
            var act = async () => await stockService.CreateBuyOrderAsync(null!);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100001)]
        public async Task CreateBuyOrder_QuantityIsOutOfBound(uint quantity)
        {
            SetupBuyMocks();
            var buyOrder = _fixture.Build<OrderRequest>()
                .With(bor => bor.Quantity, (uint)quantity)
                .Create();

            var act = async () => await stockService.CreateBuyOrderAsync(buyOrder);

            await act.Should().ThrowAsync<ArgumentException>();
            _mockStockRepository.Verify(rep => rep.CreateBuyOrderAsync(It.IsAny<BuyOrder>()), Times.Never());
        }

        [Fact]
        public async Task CreateBuyOrder_StockSymbolIsNull()
        {
            SetupBuyMocks();
            var buyOrder = _fixture.Build<OrderRequest>()
                .With(bor => bor.StockSymbol, null as string)
                .Create();

            var act = async () => await stockService.CreateBuyOrderAsync(buyOrder);

            await act.Should().ThrowAsync<ArgumentException>();
            _mockStockRepository.Verify(rep => rep.CreateBuyOrderAsync(It.IsAny<BuyOrder>()), Times.Never());
        }

        [Fact]
        public async Task CreateBuyOrder_CorrectUse()
        {
            SetupBuyMocks();

            var buyOrder = _fixture.Create<OrderRequest>();

            await stockService.CreateBuyOrderAsync(buyOrder);

            _mockStockRepository.Verify();
        }

        private void SetupBuyMocks()
        {
            _mockFinnHubService.Setup(fh => fh.GetStockPriceQuote(It.IsAny<string>()))
               .ReturnsAsync(new Dictionary<string, object> { { "c", 10 } });

            _mockMapper.Setup(mapper => mapper.Map<BuyOrder>(It.IsAny<IOrderRequest>()))
                .Returns(_fixture.Create<BuyOrder>());

            _mockStockRepository.Setup(rep => rep.CreateBuyOrderAsync(It.IsAny<BuyOrder>())).Verifiable();
        }
        #endregion

        #region CreateSellOrder
        [Fact]
        public async Task CreateSellOrder_SellOrderIsNull()
        {
            SetupSellMocks();

            var act = async () => await stockService.CreateSellOrderAsync(null!);

            await act.Should().ThrowAsync<ArgumentNullException>();
            _mockStockRepository.Verify(rep => rep.CreateSellOrderAsync(It.IsAny<SellOrder>()), Times.Never());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100001)]
        public async Task CreateSellOrder_QuantityIsOutOfBound(uint quantity)
        {
            SetupSellMocks();

            var SellOrder = _fixture.Build<OrderRequest>()
                .With(bor => bor.Quantity, (uint)quantity)
                .Create();

            var act = async () => await stockService.CreateSellOrderAsync(SellOrder);

            await act.Should().ThrowAsync<ArgumentException>();
            _mockStockRepository.Verify(rep => rep.CreateSellOrderAsync(It.IsAny<SellOrder>()), Times.Never());
        }

        [Fact]
        public async Task CreateSellOrder_StockSymbolIsNull()
        {
            var SellOrder = _fixture.Build<OrderRequest>()
                .With(bor => bor.StockSymbol, null as string)
                .Create();

            var act = async () => await stockService.CreateSellOrderAsync(SellOrder);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateSellOrder_CorrectUse()
        {
            SetupSellMocks();
            _mockFinnHubService.Setup(fh => fh.GetStockPriceQuote(It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, object> { { "c", 10 } });

            var SellOrder = _fixture.Create<OrderRequest>();

            await stockService.CreateSellOrderAsync(SellOrder);
            _mockStockRepository.Verify(rep => rep.CreateSellOrderAsync(It.IsAny<SellOrder>()), Times.Once());
        }

        private void SetupSellMocks()
        {
            _mockFinnHubService.Setup(fh => fh.GetStockPriceQuote(It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, object> { { "c", 10 } });

            _mockMapper.Setup(mapper => mapper.Map<SellOrder>(It.IsAny<IOrderRequest>()))
                .Returns(_fixture.Create<SellOrder>());

            _mockStockRepository.Setup(rep => rep.CreateBuyOrderAsync(It.IsAny<BuyOrder>())).Verifiable();
        }
        #endregion

        #region GetBuyOrders
        [Fact]
        public async Task GetBuyOrders_AfterAddAFewOrders()
        {
            _mockMapper.Setup(mapper => mapper.Map<OrderResponse>(It.IsAny<BuyOrder>()))
                .Returns(_fixture.Create<OrderResponse>());

            var buyRequests = _fixture.Create<List<OrderRequest>>();

            _mockStockRepository.Setup(sr => sr.GetBuyOrdersAsync())
                .ReturnsAsync(buyRequests.Select(or => _fixture.Build<BuyOrder>()
                .With(bo => bo.StockSymbol, or.StockSymbol)
                .Create())
                .Take(2)
                .ToList()).Verifiable();

            var orders = await stockService.GetBuyOrdersAsync();
            orders.Should().HaveCount(2);
            _mockStockRepository.Verify();
        }

        [Fact]
        public async Task GetBuyOrders_BuyOrdersIsEmpty()
        {
            _mockStockRepository.Setup(sr => sr.GetBuyOrdersAsync())
                .ReturnsAsync(new List<BuyOrder>());

            var response = await stockService.GetBuyOrdersAsync();

            response.Should().BeEmpty();
        }
        #endregion

        #region GetSellOrders
        [Fact]
        public async Task GetSellOrders_BuyOrdersIsEmpty()
        {
            _mockStockRepository.Setup(sr => sr.GetSellOrdersAsync())
                .ReturnsAsync(new List<SellOrder>());

            var response = await stockService.GetSellOrdersAsync();

            response.Should().BeEmpty();
        }

        [Fact]
        public async Task GetSellOrders_AfterAddAFewOrders()
        {
            _mockMapper.Setup(mapper => mapper.Map<OrderResponse>(It.IsAny<SellOrder>()))
                .Returns(_fixture.Create<OrderResponse>());

            var sellRequests = _fixture.Create<List<OrderRequest>>();

            _mockStockRepository.Setup(sr => sr.GetSellOrdersAsync())
                .ReturnsAsync(sellRequests.Select(or => _fixture.Build<SellOrder>()
                .With(so => so.StockSymbol, or.StockSymbol)
                .Create())
                .Take(2)
                .ToList()).Verifiable();

            var orders = await stockService.GetSellOrdersAsync();
            orders.Should().HaveCount(2);
            _mockStockRepository.Verify();
        }
        #endregion
    }
}