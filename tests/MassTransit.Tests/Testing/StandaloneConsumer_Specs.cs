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
namespace MassTransit.Tests.Testing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class StandaloneConsumer_Specs
    {
        [Test]
        public async Task Should_be_able_to_create_standalone_consumer_test_in_memory()
        {
            var harness = new InMemoryTestHarness();
            ConsumerTestHarness<MyConsumer> consumer = harness.Consumer<MyConsumer>();

            await harness.Start();
            try
            {
                await harness.InputQueueSendEndpoint.Send(new PingMessage());

                Assert.That(harness.Consumed.Select<PingMessage>().Any(), Is.True);

                Assert.That(consumer.Consumed.Select<PingMessage>().Any(), Is.True);
            }
            finally
            {
                await harness.Stop();
            }
        }


        class MyConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Console.Out.WriteLineAsync("Pinged");
            }
        }
    }
}