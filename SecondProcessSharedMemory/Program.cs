#if DEBUG

SharedMemory.Infra.ProcessStarter.StartClient();

#else

SharedMemory.Infra.ProcessStarter.WaitCanStart(false);
_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<SecondProcessSharedMemory.Benchmark.ClientBenchmark>();

#endif