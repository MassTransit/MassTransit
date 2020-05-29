namespace MassTransit.Tests.Diagnostics
{
    using System;
    using Monitoring.Performance.StatsD;
    using NUnit.Framework;


    public class StatsD_Specs
    {
        [Test]
        [Explicit]
        public void RunSomeDataIn()
        {
            var r = new Random();
            using (var spc = new StatsDPerformanceCounter(new StatsDConfiguration("10.211.55.2", 8125), "test", "test", "test"))
            {
                var start = DateTime.Now;
                var iterations = 100000;

                for (var i = 0; i < iterations; i++)
                    spc.Increment();

                var end = DateTime.Now;

                var totalSeconds = (end - start).TotalSeconds;
                Console.WriteLine("Took {0} seconds ({1}/sec)", totalSeconds, iterations / totalSeconds);
            }
        }
    }
}
