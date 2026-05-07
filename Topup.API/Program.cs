using Microsoft.EntityFrameworkCore;
using Topup.API.Middleware;
using Topup.Application.Features.Purchase.Command;
using Topup.Application.Features.Topup;
using Topup.Application.Features.Transactions.Queries;
using Topup.Application.Interfaces;
using Topup.Infrastructure.ExternalServices;
using Topup.Infrastructure.Persistence;
using Topup.Infrastructure.Queue;
using Topup.Infrastructure.Services;
using Topup.Infrastructure.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("TopupDb");
});

builder.Services.AddScoped<IApplicationDbContext>(
    provider =>
        provider.GetRequiredService<AppDbContext>());

// Add services to the container.
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(PurchaseCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetTransactionQuery).Assembly);
});

builder.Services.AddSingleton<InMemoryQueue<string>>();

builder.Services.AddHostedService<TopupWorker>();

builder.Services.AddSingleton<IMciClient,MciClient>();

builder.Services.AddScoped<ISwitchService, SwitchService>();

builder.Services.AddScoped<TopupSagaOrchestrator>();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();

app.UseSwaggerUI();

app.MapControllers();

app.Run();