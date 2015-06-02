// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture, Explicit]
    public class Pounding_the_crap_out_of_the_send_endpoint :
        RabbitMqTestFixture
    {
        TaskCompletionSource<int> _completed;

        public Pounding_the_crap_out_of_the_send_endpoint()
        {
            TestTimeout = TimeSpan.FromSeconds(180);
        }

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _completed = GetTask<int>();
            int count = 0;

            configurator.PrefetchCount = 1000;

            configurator.Handler<PingMessage>(async context =>
            {
                if (Interlocked.Increment(ref count) == 100000)
                    _completed.TrySetResult(count);
            });
        }

        [Test]
        public async void Should_end_well()
        {
            var timer = Stopwatch.StartNew();

            Task[] publishers = new Task[100*1000];
            Parallel.For(0, 100, i =>
            {
                var offset = i * 1000;

                for (int j = 0; j < 1000; j++)
                {
                    var ping = new PingMessage();
                    var task = Bus.Publish(ping);
                    publishers[offset + j] = task;
                }
            });

            var published = timer.Elapsed;

            await Task.WhenAll(publishers);

            var confirmed = timer.Elapsed;

            Console.WriteLine("Published {0} messages in {1}ms ({2:F0}/s)", 100 * 1000, published.TotalMilliseconds, 100L * 1000L * 1000L / published.TotalMilliseconds);

            Console.WriteLine("Confirmed {0} messages in {1}ms ({2:F0}/s)", 100 * 1000, confirmed.TotalMilliseconds, 100L * 1000L * 1000L / confirmed.TotalMilliseconds);

            await _completed.Task;

            var completed = timer.Elapsed;

            Console.WriteLine("Completed {0} messages in {1}ms ({2:F0}/s)", 100 * 1000, completed.TotalMilliseconds, 100L * 1000L * 1000L / completed.TotalMilliseconds);
        }
    }
}