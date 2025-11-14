using BenchmarkDotNet.Running;

using Benchmarks;

var customConfig = new Config();
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, customConfig);