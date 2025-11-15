using System.Collections.Immutable;

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

using Perfolizer.Horology;

using Perfolizer.Mathematics.OutlierDetection;

namespace Benchmarks;

public class Config : ManualConfig {
    public Config() {
        SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
        AddDiagnoser(MemoryDiagnoser.Default);
        AddJob(Job.Default
            .WithOutlierMode(OutlierMode.DontRemove)
            .WithLaunchCount(3)
            .WithWarmupCount(5)
            .WithIterationCount(20)
            .WithIterationTime(TimeInterval.FromMilliseconds(500)));
        AddColumnProvider(DefaultColumnProviders.Instance);
        HideColumns(Column.Error, Column.StdDev, Column.Median, Column.RatioSD);
        WithOrderer(new GroupByTypeOrderer());
        WithOptions(ConfigOptions.JoinSummary);
        WithOptions(ConfigOptions.StopOnFirstError);
        WithOptions(ConfigOptions.DisableLogFile);
        AddExporter(MarkdownExporter.GitHub);
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
        cases.OrderBy(c => c.Descriptor.Type.FullName)
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