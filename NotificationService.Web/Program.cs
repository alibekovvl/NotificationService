using Hangfire;
using Hangfire.MemoryStorage;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using MockService.Services;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Mapping;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Hubs;
using NotificationService.Infrastructure.Repositories;
using NotificationService.Infrastructure.Services;
using NotificationService.Infrastructure.Services.Caching;

var builder = WebApplication.CreateBuilder(args);
var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");


builder.Services.AddSignalR();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Укажи адрес фронтенда
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();  // Разрешаем использование cookies и credentials
    });
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
    .AddScoped<IAiAnalysisService, MockAiAnalysisService>()
    .AddScoped<INotificationBroadcaster, NotificationBroadcaster>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
app.MapHub<NotificationHub>("/notificationHub");
app.UseHangfireDashboard("/hangfire");
app.UseAuthorization();
app.MapControllers();
app.Run();
