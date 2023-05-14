using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Infra.Shared.Benchmark;
using SharedMemory.Infra;

namespace FirstProcessSharedMemory.Benchmark;

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