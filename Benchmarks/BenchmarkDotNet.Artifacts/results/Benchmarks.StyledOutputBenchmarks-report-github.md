```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.3.1 (a) (25D771280a) [Darwin 25.3.0]
Apple M2 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.201
  [Host] : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  PGO1   : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a

Job=PGO1  OutlierMode=RemoveAll  EnvironmentVariables=DOTNET_TieredPGO=1
IterationCount=30  IterationTime=100ms  LaunchCount=3
WarmupCount=5

```
| Method         | Mean        | Ratio         | Gen0   | Allocated | Alloc Ratio   |
|--------------- |------------:|--------------:|-------:|----------:|--------------:|
| PrettyConsole  |    53.10 ns | 92.14x faster |      - |         - |            NA |
| SpectreConsole | 4,889.25 ns |      baseline | 2.0880 |   17840 B |               |
| SystemConsole  |    70.48 ns | 69.37x faster | 0.0022 |      24 B | 743.333x less |
