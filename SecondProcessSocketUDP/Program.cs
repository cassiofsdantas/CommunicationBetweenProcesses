#if DEBUG

SocketUDP.Infra.ProcessStarter.StartClient();

#else

SocketUDP.Infra.ProcessStarter.WaitCanStart(false);
_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<SecondProcessSocketUDP.Benchmark.ClientBenchmark>();

#endif