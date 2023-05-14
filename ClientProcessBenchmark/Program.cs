using BenchmarkDotNet.Attributes;
using Infra.Shared.Benchmark;

ProcessWaiter.WaitCanStart(false);
_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<ClientBenchmark>();


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
public class ClientBenchmark
{
    [Benchmark]
    public void StartClientMMAP()
    {
        MMAP.Infra.ProcessStarter.StartClient();
    }

    [Benchmark]
    public void StartClientSharedFile()
    {
        SharedFile.Infra.ProcessStarter.StartClient();
    }

    [Benchmark]
    public void StartClientSharedMemory()
    {
        SharedMemory.Infra.ProcessStarter.StartClient();
    }

    [Benchmark(Baseline = true)]
    public void StartClientSocketTCP()
    {
        SocketTCP.Infra.ProcessStarter.StartClient();
    }

    [Benchmark]
    public void StartClientSocketUDP()
    {
        SocketUDP.Infra.ProcessStarter.StartClient();
        SocketUDP.Infra.ProcessStarter.WaitCanStart(false);
    }
}