using Microsoft.EntityFrameworkCore;
using Switch.Api;
using Switch.Api.Persistence;
using System;
using System.Net.Sockets;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("TopupDb"));

    services.AddSingleton<InMemoryQueue<string>>();

    services.AddSingleton<MciClient>();

    services.AddHostedService<TopupWorker>();
});

await builder.RunConsoleAsync();