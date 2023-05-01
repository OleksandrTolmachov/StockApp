using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockApp.Application.Options;
using StockApp.Application.RepositoryContracts;
using StockApp.Application.Services;
using StockApp.Infrastructure.Mappings;
using StockApp.Infrastructure.Repositories;

namespace StockApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllersWithViews();
        services.AddScoped<IFinnHubService, FinnHubService>();
        services.AddScoped<IStockService, StockService>();

        services.AddScoped<IFinnhubRepository, FinnhubRepository>();
        services.AddScoped<IStocksRepository, StocksRepository>();

        services.AddHttpClient("Finnhub", httpClient =>
        {
            httpClient.BaseAddress = new Uri(config.GetValue<string>("FinnhubBaseUrl"));
        });

        services.AddOptions<StockOptions>()
            .Bind(config.GetSection("TradingOptions"))
            .ValidateDataAnnotations();
        services.Configure<TokenOption>(config.GetSection("TokenOptions"));
        services.AddDbContext<OrdersDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("Default"));
        });
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders
            | HttpLoggingFields.ResponsePropertiesAndHeaders;
        });
        services.AddAutoMapper(typeof(OrderProfile).Assembly);

        return services;
    }
}
