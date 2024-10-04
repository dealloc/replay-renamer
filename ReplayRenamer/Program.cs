using GBX.NET;
using GBX.NET.Engines.Game;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReplayRenamer;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<ReplayRenamerService>();

await builder
    .Build()
    .RunAsync();

var files = Directory.GetFiles(".", "*.gbx", SearchOption.TopDirectoryOnly);

foreach (var file in files)
{
    var header = Gbx.ParseHeaderNode(file);

    if (header is CGameCtnReplayRecord replay)
    {
        
    }
}