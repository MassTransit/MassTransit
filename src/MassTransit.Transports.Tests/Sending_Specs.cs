// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.Tests
{
    namespace Sending
    {
        using System;
        using System.Linq;
        using System.Threading.Tasks;
        using NUnit.Framework;
        using Testing;


        public interface TestCommand
        {
            Guid CorrelationId { get; }
        }


        public class Sending_a_message_to_an_endpoint :
            TransportTest
        {
            [Test]
            public async Task Should_be_handled_by_the_consumer()
            {
                var correlationId = NewId.NextGuid();

                await Harness.InputQueueSendEndpoint.Send<TestCommand>(new
                {
                    CorrelationId = correlationId
                });

                Assert.That(_consumer.Consumed.Select<TestCommand>().Any(), Is.True);

                IReceivedMessage<TestCommand> message = _consumer.Consumed.Select<TestCommand>().First();

                Assert.That(message.Context.CorrelationId, Is.EqualTo(correlationId));
            }

            ConsumerTestHarness<TestCommandConsumer> _consumer;

            public Sending_a_message_to_an_endpoint(Type harnessType)
                : base(harnessType)
            {
                ConfigureConsumeTopology = false;
            }

            [OneTimeSetUp]
            public Task Setup()
            {
                _consumer = Harness.Consumer<TestCommandConsumer>();

                return Harness.Start();
            }

            [OneTimeTearDown]
            public Task Stop()
            {
                return Harness.Stop();
            }


            class TestCommandConsumer :
                IConsumer<TestCommand>
            {
                public Task Consume(ConsumeContext<TestCommand> context)
                {
                    return Task.CompletedTask;
                }
            }
        }
    }
}