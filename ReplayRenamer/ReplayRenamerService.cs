using System.Xml.Linq;
using GBX.NET;
using GBX.NET.Engines.Game;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ReplayRenamer;

public class ReplayRenamerService : BackgroundService
{
    private readonly ILogger<ReplayRenamerService> _logger;
    private readonly IHostApplicationLifetime _lifetime;

    private readonly string _filePattern;
    private readonly string _fileNameTemplate;

    public ReplayRenamerService(
        ILogger<ReplayRenamerService> logger,
        IConfiguration configuration,
        IHostApplicationLifetime lifetime
    )
    {
        _logger = logger;
        _lifetime = lifetime;

        _filePattern = configuration.GetValue<string>("ReplayRenamer:FilePattern") ?? "*.gbx";
        _fileNameTemplate = configuration.GetValue<string>("ReplayRenamer:FileNameTemplate") ?? "{Original}";
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var inputs = Environment
            .GetCommandLineArgs()
            .Skip(1);
        
        _logger.LogDebug("Starting renamer with following inputs: {Inputs}", inputs);

        foreach (var (path, replay) in GetReplays(inputs))
        {
            if (replay is not null)
            {
                var folder = Path.GetDirectoryName(path);
                var original = Path.GetFileNameWithoutExtension(path);
                var extension = Path.GetExtension(path);

                var target = GetFileName(replay, original);
                var targetPath = $"{Path.Combine(folder!, target)}{extension}";
                _logger.LogInformation("Renaming {Path} to {Target}", original, target);
                _logger.LogDebug("MOVE {Path} {Target}", path, targetPath);
                
                File.Move(path, targetPath);
            }
        }
        
        _logger.LogInformation("Renaming finished, shutting down");
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        _lifetime.StopApplication();
    }

    private IEnumerable<(string Path, CGameCtnReplayRecord? Replay)> GetReplays(IEnumerable<string> inputs)
    {
        foreach (var input in inputs)
        {
            if (Directory.Exists(input))
            {
                var files = Directory.EnumerateFiles(input, _filePattern, SearchOption.TopDirectoryOnly);

                foreach (var file in files)
                {
                    yield return GetReplay(file);
                }
            }
            else if (File.Exists(input))
            {
                yield return GetReplay(input);
            }
            else
            {
                _logger.LogWarning("Input file {Input} does not exist", input);
            }
        }
    }

    private (string Path, CGameCtnReplayRecord? Replay) GetReplay(string path)
    {
        var header = Gbx.ParseHeaderNode(path);

        if (header is CGameCtnReplayRecord replay)
        {
            return (path, replay);
        }
        
        _logger.LogWarning("Input file {Path} does not contain GBX header or is not a valid TrackMania file", path);
        return (path, null);
    }

    private string GetFileName(CGameCtnReplayRecord replay, string filename)
    {
        var element = XElement.Parse(replay.Xml!);
        var mapName = element.Element("map")?.Attribute("name")?.Value;

        var name = _fileNameTemplate
            .Replace("{Time}", replay.Time?.ToString())
            .Replace("{Player}", replay.PlayerNickname)
            .Replace("{Login}", replay.PlayerLogin)
            .Replace("{Track}", mapName)
            .Replace("{Original}", filename);

        return string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
    }
}