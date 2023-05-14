#if DEBUG

MMAP.Infra.ProcessStarter.StartClient();

#else

MMAP.Infra.ProcessStarter.WaitCanStart(false);
_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<SecondProcessMMAP.Benchmark.ClientBenchmark>();

#endif