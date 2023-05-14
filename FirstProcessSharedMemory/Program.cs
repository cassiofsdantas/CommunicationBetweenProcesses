#if DEBUG

SharedMemory.Infra.ProcessStarter.StartServer();

#else

SharedMemory.Infra.ProcessStarter.WaitCanStart(true);
_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<FirstProcessSharedMemory.Benchmark.ServerBenchmark>();

#endif