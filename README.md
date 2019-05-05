# FindDupes
[![NuGet](https://img.shields.io/nuget/v/FindDupes.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/FindDupes/)

 A fast and versatile file duplication finder.

### Features
- Uses file hash, size and timestamp (optional) as filtering criteria
- Switch between SHA1 and xxHash for hashing
- Multi-threaded
- Supports dry-run mode
- Supports progress bar
- Can output timing information to benchmark disk speeds

### Install
You can install it as a dotnet tool like this:

```PowerShell
dotnet tool install --global FindDupes
```

There are lots of command switches to choose from.
```
# FindDupes --help

FindDupes 1.0.0
Copyright Ian Qvist 2019

  --disable-size        Do not use size as a criteria for comparing duplicates.
  --enable-timestamp    Use timestamp as a criteria for comparing duplicates.
  --disable-hash        Do not use hash as a criteria for comparing duplicates.
  --min-size            The minimum size in bytes as a filter for duplicates.
  --max-size            (Default: 9223372036854775807) The maximum size in bytes as a filter for duplicates.
  -f, --fasthash        Use a faster hash function.
  --skip-hidden         Skip hidden files.
  --no-ask              Don't ask to delete files, just delete them.
  -t, --threads         The number of threads to use. By default it uses all available cores.
  -p, --progress        Show progress bar.
  --timing              Show timing information.
  -r, --recursive       Go into sub-directories recursively.
  --dry                 Do a dry-run. Don't actually delete files.
  --max-files           (Default: 1) Maximum number of files to keep in memory at once.
  --help                Display this help screen.
  --version             Display version information.

  Directory (pos. 0)    Required. The directory to remove duplicates from.
```