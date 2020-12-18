using BenchmarkDotNet.Running;

namespace DistinctByTest
{
    static class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}
