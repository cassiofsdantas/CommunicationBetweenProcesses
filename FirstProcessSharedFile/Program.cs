#if DEBUG

SharedFile.Infra.ProcessStarter.StartServer();

#else

SharedFile.Infra.ProcessStarter.WaitCanStart(true);
_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<FirstProcessSharedFile.Benchmark.ServerBenchmark>();

#endif