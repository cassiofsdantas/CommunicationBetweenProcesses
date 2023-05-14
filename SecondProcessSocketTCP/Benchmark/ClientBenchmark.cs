using BenchmarkDotNet.Attributes;
using Infra.Shared.Benchmark;
using SocketTCP.Infra;

namespace SecondProcessSocketTCP.Benchmark;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
[RankColumn, MinColumn, MaxColumn, Q1Column, Q3Column, AllStatisticsColumn]
[JsonExporterAttribute.Full, CsvMeasurementsExporter]
public class ClientBenchmark
{
    [Benchmark]
    public void StartClient()
    {
        ProcessStarter.StartClient();
    }
}