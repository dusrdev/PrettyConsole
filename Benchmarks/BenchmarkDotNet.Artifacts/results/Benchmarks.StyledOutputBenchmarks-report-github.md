```

BenchmarkDotNet v0.15.6, macOS 26.1 (25B78) [Darwin 25.1.0]
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
| PrettyConsole  |    58.34 ns | 86.94x faster |      - |         - |            NA |
| SpectreConsole | 5,069.69 ns |      baseline | 2.1284 |   17840 B |               |
| SystemConsole  |    71.82 ns | 70.59x faster | 0.0022 |      24 B | 743.333x less |
