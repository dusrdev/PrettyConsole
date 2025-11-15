```

BenchmarkDotNet v0.15.6, macOS 26.1 (25B78) [Darwin 25.1.0]
Apple M2 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  Job-GIBNDH : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

OutlierMode=DontRemove  IterationCount=20  IterationTime=500ms  
LaunchCount=3  WarmupCount=5  

```
| Method         | Mean        | Ratio         | Gen0   | Gen1   | Allocated | Alloc Ratio   |
|--------------- |------------:|--------------:|-------:|-------:|----------:|--------------:|
| PrettyConsole  |    95.06 ns | 49.95x faster |      - |      - |         - |            NA |
| SpectreConsole | 4,747.67 ns |      baseline | 2.1255 | 0.0191 |   17840 B |               |
| SystemConsole  |    67.90 ns | 69.92x faster | 0.0028 |      - |      24 B | 743.333x less |
