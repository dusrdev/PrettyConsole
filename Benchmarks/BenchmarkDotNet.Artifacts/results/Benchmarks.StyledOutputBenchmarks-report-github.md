```

BenchmarkDotNet v0.15.6, macOS 26.1 (25B78) [Darwin 25.1.0]
Apple M2 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  Job-BGFQEO : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

OutlierMode=DontRemove  IterationCount=30  IterationTime=100ms  
LaunchCount=3  WarmupCount=5  

```
| Method         | Mean        | Ratio         | Gen0   | Allocated | Alloc Ratio   |
|--------------- |------------:|--------------:|-------:|----------:|--------------:|
| PrettyConsole  |    94.37 ns | 49.96x faster |      - |         - |            NA |
| SpectreConsole | 4,713.76 ns |      baseline | 2.1278 |   17840 B |               |
| SystemConsole  |    68.77 ns | 68.55x faster | 0.0028 |      24 B | 743.333x less |
