FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/PricingEngine.Api/PricingEngine.Api.csproj",                                     "src/PricingEngine.Api/"]
COPY ["src/PricingEngine.Web/PricingEngine.Web.csproj",                                     "src/PricingEngine.Web/"]
COPY ["src/PricingEngine.Infrastructure/PricingEngine.Infrastructure.csproj",               "src/PricingEngine.Infrastructure/"]
COPY ["src/PricingEngine.Application/PricingEngine.Application.csproj",                     "src/PricingEngine.Application/"]
COPY ["src/PricingEngine.Domain/PricingEngine.Domain.csproj",                               "src/PricingEngine.Domain/"]
RUN dotnet restore "src/PricingEngine.Api/PricingEngine.Api.csproj"
COPY . .
WORKDIR "/src/src/PricingEngine.Api"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PricingEngine.Api.dll"]
