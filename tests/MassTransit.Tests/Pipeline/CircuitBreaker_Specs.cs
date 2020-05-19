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
namespace MassTransit.Tests.Pipeline
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class Specifying_a_circuit_breaker
    {
        [Test]
        public async Task Should_allow_the_first_call()
        {
            var count = 0;
            IPipe<ConsumeContext<Running_two_in_memory_transports.A>> pipe = Pipe.New<ConsumeContext<Running_two_in_memory_transports.A>>(x =>
            {
                x.UseCircuitBreaker(v =>
                {
                    v.ResetInterval = TimeSpan.FromSeconds(60);
                    v.Handle<IntentionalTestException>();
                });
                x.UseExecute(payload =>
                {
                    Interlocked.Increment(ref count);

                    throw new IntentionalTestException();
                });
            });

            var context = new TestConsumeContext<Running_two_in_memory_transports.A>(new Running_two_in_memory_transports.A());

            for (var i = 0; i < 100; i++)
                Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            count.ShouldBe(6);
        }
    }
}