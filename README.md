# Replay Renamer

A small, simple tool to rename your TrackMania files.
Usage is quite simple, download the latest version from [releases](https://github.com/dealloc/replay-renamer/releases).
Then, drag a `.gbx` file or a folder containing `.gbx` files on the executable.

You should get a console window, and the files should be renamed according to the configured naming convention.

### Configuring names (and other stuff)
Alongside the executable you should find a file called `appsettings.json` (or just `appsettings` if your computer does not show file extensions).
If you open this file in notepad you'll see something along the lines of this:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Trace"
    },
    "File": {
      "Path": "replay-renamer.log",
      "Append": true,
      "MinLevel": "Trace",
      "FileSizeLimitBytes": 1024,
      "MaxRollingFiles": 5
    }
  },
  "ReplayRenamer": {
    "FilePattern": "*.gbx",
    "FileNameTemplate": "[{Time}] - [{Player}] - [{Track}]"
  }
}
```

You generally only want to change the name of `FileNameTemplate`, and make sure that you leave all quotation marks (`"`) in place!
Names surrounded with `{}` (like `{Time}`, `{Player}`, ...) will be replaced with the values from your game.
Currently, the following names are available (capital sensitive!):
- `{Time}` (track time)
- `{Player}` (player nickname)
- `{Login}` (player login)
- `{Track}` (name of the track)
- `{Original}` (original filename)