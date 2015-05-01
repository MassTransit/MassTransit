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
namespace MassTransit.Tests.Pipeline
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using Transports.InMemoryTransport_Specs;


    [TestFixture]
    public class Specifying_a_concurrency_limit
    {
        [Test, Explicit]
        public async void Should_prevent_too_many_threads_at_one_time()
        {
            int currentCount = 0;
            int maxCount = 0;

            IPipe<ConsumeContext<A>> pipe = Pipe.New<ConsumeContext<A>>(x =>
            {
                x.UseConcurrencyLimit(1);
                x.ExecuteAsync(async payload =>
                {
                    int current = Interlocked.Increment(ref currentCount);
                    while (current > maxCount)
                        Interlocked.CompareExchange(ref maxCount, current, maxCount);

                    await Task.Delay(10);

                    Interlocked.Decrement(ref currentCount);
                });
            });

            var context = new TestConsumeContext<A>(new A());

            Task[] tasks = Enumerable.Range(0, 50)
                .Select(index => Task.Run(async () => await pipe.Send(context)))
                .ToArray();

            await Task.WhenAll(tasks);

            maxCount.ShouldBe(1);
        }

        [Test, Explicit]
        public async void Should_allow_just_enough_threads_at_once()
        {
            int currentCount = 0;
            int maxCount = 0;

            IPipe<ConsumeContext<A>> pipe = Pipe.New<ConsumeContext<A>>(x =>
            {
                x.UseConcurrencyLimit(32);
                x.ExecuteAsync(async payload =>
                {
                    int current = Interlocked.Increment(ref currentCount);
                    while (current > maxCount)
                        Interlocked.CompareExchange(ref maxCount, current, maxCount);

                    await Task.Delay(10);

                    Interlocked.Decrement(ref currentCount);
                });
            });

            var context = new TestConsumeContext<A>(new A());

            Task[] tasks = Enumerable.Range(0, 500)
                .Select(index => Task.Run(async () => await pipe.Send(context)))
                .ToArray();

            await Task.WhenAll(tasks);

            maxCount.ShouldBe(32);
        }
    }

    [TestFixture]
    public class Specifying_a_rate_limit
    {
        [Test, Explicit]
        public async void Should_only_do_n_messages_per_interval()
        {
            int count = 0;
            IPipe<ConsumeContext<A>> pipe = Pipe.New<ConsumeContext<A>>(x =>
            {
                x.UseRateLimit(10, TimeSpan.FromSeconds(1));
                x.Execute(payload =>
                {
                    Interlocked.Increment(ref count);
                });
            });

            var context = new TestConsumeContext<A>(new A());

            var timer = Stopwatch.StartNew();

            Task[] tasks = Enumerable.Range(0, 101)
                .Select(index => Task.Run(async () => await pipe.Send(context)))
                .ToArray();

            await Task.WhenAll(tasks);

            timer.Stop();

            timer.ElapsedMilliseconds.ShouldBeGreaterThan(9500);
        }
    }
}