using System.Collections.Immutable;

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.NativeAot;

using Perfolizer.Horology;
using Perfolizer.Mathematics.OutlierDetection;

namespace Benchmarks;

public class Config : ManualConfig {
    public Config() {
        UnionRule = ConfigUnionRule.AlwaysUseLocal;
        SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
        AddDiagnoser(MemoryDiagnoser.Default);
        var baseJob = Job.Default
            .WithId("PGO1")
            .WithOutlierMode(OutlierMode.RemoveAll)
            .WithLaunchCount(3)
            .WithWarmupCount(5)
            .WithIterationCount(30)
            .WithIterationTime(TimeInterval.FromMilliseconds(100))
            .WithEnvironmentVariable("DOTNET_TieredPGO", "1"); // default, explicit for clarity

        AddJob(baseJob);
        // AddJob(baseJob
        //     .WithId("PGO2")
        //     .WithEnvironmentVariable("DOTNET_TieredPGO", "2"));
        // AddJob(baseJob
        //     .WithId("NativeAOT")
        //     .WithToolchain(NativeAotToolchain.Net10_0));
        AddColumnProvider(DefaultColumnProviders.Instance);
        HideColumns(Column.Error, Column.StdDev, Column.Median, Column.RatioSD);
        WithOrderer(new GroupByTypeOrderer());
        WithOptions(ConfigOptions.JoinSummary);
        WithOptions(ConfigOptions.StopOnFirstError);
        WithOptions(ConfigOptions.DisableLogFile);
        AddExporter(MarkdownExporter.GitHub);
        AddLogger(ConsoleLogger.Default);
    }
}

internal sealed class GroupByTypeOrderer : IOrderer {
    // Keep execution order as-declared (you can customize if you want)
    public IEnumerable<BenchmarkCase> GetExecutionOrder(
        ImmutableArray<BenchmarkCase> benchmarksCase,
        IEnumerable<BenchmarkLogicalGroupRule>? order = null)
        => benchmarksCase;

    // Sort rows in the summary: first by Type, then Method, then Params
    public IEnumerable<BenchmarkCase> GetSummaryOrder(
        ImmutableArray<BenchmarkCase> cases, Summary summary) =>
        cases.OrderBy(c => c.Job.Id)
             .ThenBy(c => c.Descriptor.Type.FullName)
             .ThenBy(c => c.Parameters.DisplayInfo);

    // We don’t use highlight groups
    public string? GetHighlightGroupKey(BenchmarkCase benchmarkCase) => null;

    // Tell BDN how to “group” rows in a joined summary (the section separator)
    public string GetLogicalGroupKey(
        ImmutableArray<BenchmarkCase> all, BenchmarkCase benchmarkCase)
        => benchmarkCase.Descriptor.Type.FullName!;

    // Order the groups themselves (by class name)
    public IEnumerable<IGrouping<string, BenchmarkCase>> GetLogicalGroupOrder(
        IEnumerable<IGrouping<string, BenchmarkCase>> logicalGroups,
        IEnumerable<BenchmarkLogicalGroupRule>? order = null)
        => logicalGroups.OrderBy(g => g.Key);

    public bool SeparateLogicalGroups => true;
}