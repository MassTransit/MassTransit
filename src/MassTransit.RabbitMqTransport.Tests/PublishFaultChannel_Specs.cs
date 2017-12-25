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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_a_fault_during_message_processing :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_have_the_original_source_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.SourceAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_move_the_message_to_the_error_queue()
        {
            await _errorHandler;

            await InputQueueSendEndpoint.Send(new PingMessage());

            await _errorHandler2;
        }

        Task<ConsumeContext<PingMessage>> _errorHandler;
        readonly Guid? _correlationId = NewId.NextGuid();
        PingMessage _pingMessage;
        PingMessage _pingMessage2;
        Task<ConsumeContext<PingMessage>> _errorHandler2;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _pingMessage = new PingMessage();
            _pingMessage2 = new PingMessage();
            await InputQueueSendEndpoint.Send(_pingMessage, Pipe.Execute<SendContext<PingMessage>>(context =>
            {
                context.CorrelationId = _correlationId;
            }));
        }

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            configurator.ReceiveEndpoint(host, "input_queue_error", x =>
            {
                x.PurgeOnStartup = true;

                _errorHandler = Handled<PingMessage>(x, context => context.Message.CorrelationId == _pingMessage.CorrelationId);
                _errorHandler2 = Handled<PingMessage>(x, context => context.Message.CorrelationId == _pingMessage2.CorrelationId);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context =>
            {
                throw new IntentionalTestException("We want to be bad, so bad");
            });
        }
    }
}