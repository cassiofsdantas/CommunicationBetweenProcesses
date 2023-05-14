#if DEBUG

SocketUDP.Infra.ProcessStarter.StartServer();

#else

SocketUDP.Infra.ProcessStarter.WaitCanStart(true);
_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<FirstProcessSocketUDP.Benchmark.ServerBenchmark>();

#endif