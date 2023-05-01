using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockApp.Infrastructure.Repositories;

namespace Tests.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            var dbDescriptor = services.FirstOrDefault(serviceDescriptor =>
            serviceDescriptor.ServiceType == typeof(DbContextOptions<OrdersDbContext>));

            if (dbDescriptor != null)
                services.Remove(dbDescriptor);

            //var mapperDescriptor = services.FirstOrDefault(serviceDescriptor =>
            //    serviceDescriptor.ServiceType == typeof(IMapper));

            //if (mapperDescriptor != null)
            //    services.Remove(mapperDescriptor);

            //services.AddAutoMapper(options =>
            //{
            //    options.CreateMap<OrderResponse, OrderPDF>().ForMember(
            //        dest => dest.OrderType,
            //        opt => opt.MapFrom(src => OrdersType.Buy));

            //    options.CreateMap<OrderResponse, OrderPDF>().ForMember(
            //        dest => dest.OrderType,
            //        opt => opt.MapFrom(src => OrdersType.Sell));

            //    options.CreateMap<IOrderRequest, SellOrder>();
            //    options.CreateMap<IOrderRequest, BuyOrder>();

            //    options.CreateMap<IOrder, OrderResponse>();
            //});

            services.AddDbContext<OrdersDbContext>(options =>
            {
                options.UseInMemoryDatabase("memoryDb");
            });

        });
    }
}
