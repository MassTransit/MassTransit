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
namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class EventPublish_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_publish_first_event()
        {
            ConsumeContext<PingReceived> received = await _received;
        }

        [Test]
        public async Task Should_publish_fourth_event()
        {
            ConsumeContext<PingCompleted> consumed = await _completed;
        }

        [Test]
        public async Task Should_publish_second_event()
        {
            ConsumeContext<PingProcessing> consumed = await _processing;
        }

        [Test]
        public async Task Should_publish_third_event()
        {
            ConsumeContext<PingConsumed> consumed = await _consumed;
        }

        Task<ConsumeContext<PingReceived>> _received;
        Task<ConsumeContext<PingConsumed>> _consumed;
        Task<ConsumeContext<PingCompleted>> _completed;
        Task<ConsumeContext<PingProcessing>> _processing;

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new Consumar());

            _received = Handled<PingReceived>(configurator);
            _processing = Handled<PingProcessing>(configurator);
            _consumed = Handled<PingConsumed>(configurator);
            _completed = Handled<PingCompleted>(configurator);
        }


        class Consumar :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await context.Publish<PingReceived>(new
                {
                    PingId = context.Message.CorrelationId,
                    Timestamp = DateTime.UtcNow
                });

                Console.WriteLine("Ping: {0}", context.Message.CorrelationId);

                await context.Publish<PingProcessing>(new
                {
                    PingId = context.Message.CorrelationId,
                    Timestamp = DateTime.UtcNow
                });

                Console.WriteLine("Processing: {0}", context.Message.CorrelationId);

                await context.Publish<PingConsumed>(new
                {
                    PingId = context.Message.CorrelationId,
                    Timestamp = DateTime.UtcNow
                });

                await context.Publish<PingCompleted>(new
                {
                    PingId = context.Message.CorrelationId,
                    Timestamp = DateTime.UtcNow,
                });
            }
        }


        public interface PingReceived
        {
            Guid PingId { get; }

            DateTime Timestamp { get; }
        }


        public interface PingProcessing
        {
            Guid PingId { get; }

            DateTime Timestamp { get; }
        }


        public interface PingConsumed
        {
            Guid PingId { get; }

            DateTime Timestamp { get; }
        }


        public interface PingCompleted
        {
            Guid PingId { get; }

            DateTime Timestamp { get; }
        }
    }
}