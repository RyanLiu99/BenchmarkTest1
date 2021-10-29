using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkTest1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Results are in C:\playground\Benchmark\BenchmarkTest1\BenchmarkTest1\bin\Release\netcoreapp3.1\BenchmarkDotNet.Artifacts\results
            //Two are indepent tests
            var summary = BenchmarkRunner.Run<PrintLineRunner>();
            var summary2 = BenchmarkRunner.Run<TempRunner>();
        }
    }

    //Other examples:
    //[ShortRunJob]

    //[Outliers(Perfolizer.Mathematics.OutlierDetection.OutlierMode.DontRemove)]

    // Skip jitting, pilot, warmup; measure 10 iterations
    //[SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Monitoring, targetCount: 10, invocationCount: 1)]

    //[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
    //[CategoriesColumn] //Ryan: display Category name in result table

    //[SimpleJob(launchCount: 1, warmupCount: 3, targetCount: 5, invocationCount: 20, id: "QuickJob")] ///specify count to make it runs faster, but not optimized

    [SimpleJob(RuntimeMoniker.NetCoreApp31, invocationCount: 50)]

    [RPlotExporter]
    [MemoryDiagnoser]
    public class PrintLineRunner
    {
        [Params(2, 10)]
        public int times;

        //[ParamsAllValues]
        //public CustomEnum E { get; set; }

        [ParamsAllValues]
        public bool? B { get; set; }

        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark(Baseline = true)]
        public void PreintDirectly()
        {
            for (int i = 0; i < times; i++)
            {
                Console.WriteLine(i);
            }
        }

        [Benchmark]
        public void PrintBufferFirst()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < times; i++)
            {
                sb.AppendLine(i.ToString());
            }
            Console.WriteLine(sb.ToString());
        }
    }
    
    [RPlotExporter]
    //Compare different run time.
    [SimpleJob(RuntimeMoniker.Net472, baseline: true)]
    [SimpleJob(runtimeMoniker: RuntimeMoniker.Mono)]
    [MemoryDiagnoser]
    public class TempRunner
    {
        //[ParamsAllValues]
        //public CustomEnum E { get; set; }

        [ParamsAllValues]
        public bool? ShouldDelay { get; set; }

        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark]
        [Arguments(90, 10)]
        [Arguments(190, 10)]
        public void Benchmark(int a, int b)
        {
            if (ShouldDelay == null || ShouldDelay == false) return;
             Task.Delay(a+b).Wait(); //Ryan: you can not test async method (e.g. make Benchmark method async), since it returns right way
        }
    }
}
