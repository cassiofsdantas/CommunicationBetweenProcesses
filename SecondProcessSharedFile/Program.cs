#if DEBUG

SharedFile.Infra.ProcessStarter.StartClient();

#else

SharedFile.Infra.ProcessStarter.WaitCanStart(false);
_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<SecondProcessSharedFile.Benchmark.ClientBenchmark>();

#endif