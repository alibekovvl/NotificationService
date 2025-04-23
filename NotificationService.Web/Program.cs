using Hangfire;
using Hangfire.MemoryStorage;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using MockService.Services;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Mapping;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Repositories;
using NotificationService.Infrastructure.Services;
using NotificationService.Infrastructure.Services.Caching;

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

builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services
    .AddScoped<INotificationRepository, NotificationRepository>()
    .AddScoped<IRedisCacheService, RedisCacheService>()
    .AddScoped<INotificationService, NotificationAppService>()
    .AddScoped<IAiAnalysisService, MockAiAnalysisService>();

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
