using Distributed.Caching.Controllers;
using Distributed.Caching.Hangfire;
using Distributed.Caching.Service;
using Hangfire;
using Hangfire.MemoryStorage;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddStackExchangeRedisCache(options => options.Configuration = "localhost:1453");


builder.Services.AddHangfire(config => config
           .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
           .UseSimpleAssemblyNameTypeSerializer()
           .UseRecommendedSerializerSettings()
           .UseMemoryStorage());


builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ICurrencyIntegrationService, CurrencyIntegrationService>();
builder.Services.AddScoped<CurrencyJob>();

builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHangfireDashboard();

// 5 saatte bir çalýþacak görevi ayarla
RecurringJob.AddOrUpdate<CurrencyJob>("update-currency", x => x.UpdateCurrencyData(), Cron.MinuteInterval(1));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
