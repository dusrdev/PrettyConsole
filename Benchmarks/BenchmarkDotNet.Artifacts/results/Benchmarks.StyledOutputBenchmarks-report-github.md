```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.1 (25B78) [Darwin 25.1.0]
Apple M2 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host] : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  PGO1   : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

Job=PGO1  OutlierMode=RemoveAll  EnvironmentVariables=DOTNET_TieredPGO=1  
IterationCount=30  IterationTime=100ms  LaunchCount=3  
WarmupCount=5  

```
| Method         | Mean        | Ratio         | Gen0   | Allocated | Alloc Ratio   |
|--------------- |------------:|--------------:|-------:|----------:|--------------:|
| PrettyConsole  |    55.96 ns | 90.23x faster |      - |         - |            NA |
| SpectreConsole | 5,046.29 ns |      baseline | 2.1193 |   17840 B |               |
| SystemConsole  |    71.64 ns | 70.44x faster | 0.0022 |      24 B | 743.333x less |
