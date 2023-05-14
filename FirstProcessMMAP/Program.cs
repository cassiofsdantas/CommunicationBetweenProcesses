#if DEBUG

MMAP.Infra.ProcessStarter.StartServer();

#else

MMAP.Infra.ProcessStarter.WaitCanStart(true);
_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<FirstProcessMMAP.Benchmark.ServerBenchmark>();

#endif
