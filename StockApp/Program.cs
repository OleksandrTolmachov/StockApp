using Rotativa.AspNetCore;
using Serilog;
using StockApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);
builder.Host.UseSerilog((contex, config) =>
{
    config.ReadFrom.Configuration(contex.Configuration);
});

if (!builder.Environment.IsEnvironment("Test"))
    RotativaConfiguration.Setup("wwwroot");

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { }
