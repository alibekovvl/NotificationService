using Hangfire;
using Hangfire.MemoryStorage;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "";
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddHangfire(x => x.UseMemoryStorage());
builder.Services.AddHangfireServer();

builder.Services
    .AddScoped<INotificationRepository, NotificationRepository>()
    .AddScoped<IRedisCacheService, RedisCacheService>()
    .AddScoped<INotificationService, NotificationAppService>()
    .AddScoped<IAIAnalysisService, MockAIAnalysisService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard("/hangfire");
app.UseAuthorization();
app.MapControllers();
app.Run();
