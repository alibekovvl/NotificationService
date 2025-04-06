using Hangfire;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// builder.Services.AddHangfire();
builder.Services
    .AddScoped<INotificationRepository, NotificationRepository>()
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
app.UseAuthorization();
app.MapControllers();
app.Run();
