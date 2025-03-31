using Com.Store.Orders.Api.Extensions;
using Com.Store.Orders.Api.Middlewares;
using Com.Store.Orders.Domain.Data;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Host.UseNLog();
LogManager.Setup().LoadConfigurationFromAppSettings();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseOutputCache();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

app.Run();
public partial class Program { }