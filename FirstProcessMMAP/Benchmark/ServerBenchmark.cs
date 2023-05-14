using BenchmarkDotNet.Attributes;
using Infra.Shared.Benchmark;
using MMAP.Infra;

namespace FirstProcessMMAP.Benchmark;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
[RankColumn, MinColumn, MaxColumn, Q1Column, Q3Column, AllStatisticsColumn]
[JsonExporterAttribute.Full, CsvMeasurementsExporter]
public class ServerBenchmark
{
    [Benchmark]
    public void StartServer()
    {
        ProcessStarter.StartServer();
    }
}