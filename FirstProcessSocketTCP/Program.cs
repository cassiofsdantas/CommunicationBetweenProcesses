#if DEBUG

SocketTCP.Infra.ProcessStarter.StartServer();

#else

SocketTCP.Infra.ProcessStarter.WaitCanStart(true);
_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<FirstProcessSocketTCP.Benchmark.ServerBenchmark>();

#endif