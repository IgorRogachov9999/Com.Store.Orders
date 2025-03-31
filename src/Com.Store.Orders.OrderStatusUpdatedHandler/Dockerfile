FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 9000

COPY ["src/Com.Store.Orders.OrderStatusUpdatedHandler/Com.Store.Orders.OrderStatusUpdatedHandler.csproj", "src/Com.Store.Orders.OrderStatusUpdatedHandler/"]
COPY ["src/Com.Store.Orders.Domain/Com.Store.Orders.Domain.csproj", "src/Com.Store.Orders.Domain/"]
RUN dotnet restore "src/Com.Store.Orders.OrderStatusUpdatedHandler/Com.Store.Orders.OrderStatusUpdatedHandler.csproj"

COPY . .
RUN dotnet build "src/Com.Store.Orders.OrderStatusUpdatedHandler/Com.Store.Orders.OrderStatusUpdatedHandler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Com.Store.Orders.OrderStatusUpdatedHandler/Com.Store.Orders.OrderStatusUpdatedHandler.csproj" -c Release -o /app/publish /p:PublishReadyToRun=false

FROM public.ecr.aws/lambda/dotnet:8 AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
CMD ["Com.Store.Orders.OrderStatusUpdatedHandler.dll::Com.Store.Orders.OSUH.Functions::HandleAsync"]