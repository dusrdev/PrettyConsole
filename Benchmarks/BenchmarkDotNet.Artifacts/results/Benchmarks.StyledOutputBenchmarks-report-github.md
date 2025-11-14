```

BenchmarkDotNet v0.15.6, macOS 26.1 (25B78) [Darwin 25.1.0]
Apple M2 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  Job-WZRQBO : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

OutlierMode=RemoveAll  IterationCount=50  IterationTime=100ms  
LaunchCount=1  WarmupCount=10  

```
| Method         | Mean     | Ratio | Gen0   | Allocated | Alloc Ratio |
|--------------- |---------:|------:|-------:|----------:|------------:|
| PrettyConsole  | 7.575 μs |  1.00 |      - |         - |          NA |
| SpectreConsole | 7.525 μs |  1.00 | 2.1217 |   17840 B |          NA |
| SystemConsole  | 7.165 μs |  0.95 |      - |      56 B |          NA |
