#if DEBUG

SocketTCP.Infra.ProcessStarter.StartClient();

#else

SocketTCP.Infra.ProcessStarter.WaitCanStart(false);
_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<SecondProcessSocketTCP.Benchmark.ClientBenchmark>();

#endif