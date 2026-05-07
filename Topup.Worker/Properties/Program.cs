using Switch.Api;
using Microsoft.AspNetCore.RateLimiting;
using System;
using Microsoft.EntityFrameworkCore;
using Switch.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//EF InMemory
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TopupDb"));

// Queue (Singleton shared)
builder.Services.AddSingleton<InMemoryQueue<string>>();

// Workers
builder.Services.AddHostedService<OutboxWorker>();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("topup", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(1);
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRateLimiter();

app.MapControllers();

app.Run();