using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NReco.Logging.File;
using ReplayRenamer;

// When dragging files on the executable, the path gets changed
Directory.SetCurrentDirectory(AppContext.BaseDirectory);

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<ReplayRenamerService>();
builder.Services.AddLogging(logging =>
{
    logging.AddFile(builder.Configuration.GetSection("Logging"));
});


await builder
    .Build()
    .RunAsync();