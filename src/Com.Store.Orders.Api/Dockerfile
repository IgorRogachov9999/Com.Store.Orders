FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Com.Store.Orders.Api/Com.Store.Orders.Api.csproj", "src/Com.Store.Orders.Api/"]
COPY ["src/Com.Store.Orders.Domain/Com.Store.Orders.Domain.csproj", "src/Com.Store.Orders.Domain/"]
RUN dotnet restore "src/Com.Store.Orders.Api/Com.Store.Orders.Api.csproj"
COPY . .
WORKDIR "/src/src/Com.Store.Orders.Api"
RUN dotnet build "Com.Store.Orders.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Com.Store.Orders.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Com.Store.Orders.Api.dll"]