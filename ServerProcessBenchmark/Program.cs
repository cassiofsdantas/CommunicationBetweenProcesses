using BenchmarkDotNet.Attributes;
using Infra.Shared.Benchmark;

ProcessWaiter.WaitCanStart(true);
_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<ServerBenchmark>();


public static class ProcessWaiter
{
    public static readonly EventWaitHandle WaitCanStartServerEventWaitHandle = new(false, EventResetMode.ManualReset, nameof(ProcessWaiter) + nameof(WaitCanStartServerEventWaitHandle));
    public static readonly EventWaitHandle WaitCanStartClientEventWaitHandle = new(false, EventResetMode.ManualReset, nameof(ProcessWaiter) + nameof(WaitCanStartClientEventWaitHandle));

    public static void WaitCanStart(bool isServer)
    {
        if (isServer)
        {
            WaitCanStartServerEventWaitHandle.Set();
        }
        else
        {
            WaitCanStartClientEventWaitHandle.Set();
        }

        WaitHandle.WaitAll(new WaitHandle[] { WaitCanStartServerEventWaitHandle, WaitCanStartClientEventWaitHandle });
    }
}

[Config(typeof(AntiVirusFriendlyConfig))]
[RPlotExporter]
[MemoryDiagnoser]
[RankColumn, MinColumn, MaxColumn, Q1Column, Q3Column, AllStatisticsColumn]
[JsonExporterAttribute.Full, CsvMeasurementsExporter]
public class ServerBenchmark
{
    [Benchmark]
    public void StartServerMMAP()
    {
        MMAP.Infra.ProcessStarter.StartServer();
    }

    [Benchmark]
    public void StartServerSharedFile()
    {
        SharedFile.Infra.ProcessStarter.StartServer();
    }

    [Benchmark]
    public void StartServerSharedMemory()
    {
        SharedMemory.Infra.ProcessStarter.StartServer();
    }

    [Benchmark(Baseline = true)]
    public void StartServerSocketTCP()
    {
        SocketTCP.Infra.ProcessStarter.StartServer();
    }

    [Benchmark]
    public void StartServerSocketUDP()
    {
        SocketUDP.Infra.ProcessStarter.StartServer();
        SocketUDP.Infra.ProcessStarter.WaitCanStart(true);
    }
}