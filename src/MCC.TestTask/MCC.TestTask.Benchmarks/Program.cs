using BenchmarkDotNet.Running;

namespace MCC.TestTask.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<PostServiceBenchmark>();
    }
}