# FindDupes
[![NuGet](https://img.shields.io/nuget/v/FindDupes.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/FindDupes/)

 A fast and versatile file duplication finder.

### Features
- Uses hash and timestamp (both optional) as filtering criteria
- Switch between SHA1 and xxHash for hashing
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
FindDupes 1.1.0
Copyright Ian Qvist 2019

  --enable-timestamp    Use timestamps as a criteria for comparing duplicates.
  --disable-hash        Do not use hash as a criteria for comparing duplicates.
  --min-size            The minimum size in bytes as a filter for duplicates.
  --max-size            (Default: 9223372036854775807) The maximum size in bytes as a filter for duplicates.
  -f, --fasthash        Use a faster hash function.
  --skip-hidden         Skip hidden files.
  --no-ask              Don't ask to delete files, just delete them.
  -p, --progress        Show progress bar.
  --timing              Show timing information.
  -r, --recursive       Go into sub-directories recursively.
  --dry                 Do a dry-run. Don't actually delete files.
  --help                Display this help screen.
  --version             Display version information.

  Directory (pos. 0)    Required. The directory to remove duplicates from.
```