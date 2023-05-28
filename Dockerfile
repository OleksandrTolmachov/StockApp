FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5130

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY StockApp.sln ./
COPY StockApp/*.csproj ./StockApp/
COPY StockApp.Application/*.csproj ./StockApp.Application/
COPY StockApp.Domain/*.csproj ./StockApp.Domain/
COPY StockApp.Infrastrucuture/*.csproj ./StockApp.Infrastrucuture/
COPY StockApp.IntegrationTests/*.csproj ./StockApp.IntegrationTests/
COPY StockApp.UnitTests/*.csproj ./StockApp.UnitTests/

RUN dotnet restore
COPY . .
WORKDIR "/src/StockApp"
RUN dotnet build "StockApp.WebUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StockApp.WebUI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5130
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockApp.WebUI.dll"]