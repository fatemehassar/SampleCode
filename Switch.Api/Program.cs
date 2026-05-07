using Microsoft.EntityFrameworkCore;
using Switch.Api;
using Switch.Api.ExceptionHandeling;
using Switch.Api.Persistence;
using Switch.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseInMemoryDatabase("TopupDb");
});

builder.Services.AddSingleton<
    InMemoryQueue<string>>();

builder.Services.AddSingleton<MciClient>();

builder.Services.AddScoped<SwitchService>();

builder.Services.AddScoped<
    TopupSagaOrchestrator>();

builder.Services.AddHostedService<
    OutboxWorker>();

builder.Services.AddHostedService<
    TopupWorker>();

var app = builder.Build();


app.UseMiddleware<CorrelationIdMiddleware>();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();

app.UseSwaggerUI();

app.MapControllers();

app.Run();