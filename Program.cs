using System;
using System.Linq;
using System.Threading.Tasks;
using Prometheus.DotNetRuntime;

namespace LeakRepro
{
    class Program
    {
        static Task Main(string[] args)
        {
            Console.WriteLine("Started");

            switch (args.FirstOrDefault())
            {
                case "collector-enabled":
                    return RunExperiment(collectorEnabled: true);
                default:
                    return RunExperiment(collectorEnabled: false);
            }
        }

        static async Task RunExperiment(bool collectorEnabled)
        {
            if (!collectorEnabled)
            {
                Console.WriteLine(".NET runtime metric collection is DISABLED");
                await RunForcedGcLoop();
            }
            
            Console.WriteLine(".NET runtime metric collection is ENABLED");

            using var collector = DotNetRuntimeStatsBuilder.Customize()
                .WithContentionStats(SampleEvery.OneEvent)
                .WithJitStats(SampleEvery.OneEvent)
                .WithThreadPoolSchedulingStats(sampleRate: SampleEvery.OneEvent)
                .WithThreadPoolStats()
                .WithGcStats()
                .StartCollecting();

            await RunForcedGcLoop();
        }

        // Run full GC every ~100ms to have a smoother memory graph
        static async Task RunForcedGcLoop()
        {
            while (true)
            {
                GC.Collect();
                await Task.Delay(100);
            }
        }

        // Generate 10 x 1KB objects every ~1ms
        // static async Task GenerateGarbage()
        // {
        //     while (true)
        //     {
        //         for (var i = 0; i < 10; ++i)
        //         {
        //             var arr = new byte[1024];
        //             Array.Clear(arr, 0, arr.Length);
        //         }

        //         await Task.Delay(1);
        //     }
        // }
    }
}
