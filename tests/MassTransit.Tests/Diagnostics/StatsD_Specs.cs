// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
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

                for (int i = 0; i < iterations; i++)
                {
                    spc.Increment();
                }

                var end = DateTime.Now;

                var totalSeconds = (end - start).TotalSeconds;
                Console.WriteLine("Took {0} seconds ({1}/sec)", totalSeconds, iterations / totalSeconds);
            }
            
        }
    }
}