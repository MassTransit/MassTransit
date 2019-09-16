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
namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class A_serialization_exception :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_have_the_correlation_id()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.CorrelationId, Is.EqualTo(_correlationId));
        }

        [Test]
        public async Task Should_have_the_exception()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get("MT-Fault-Message", (string)null), Is.EqualTo("This is fine, forcing death"));
        }

        [Test]
        public async Task Should_have_the_host_machine_name()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get("MT-Host-MachineName", (string)null), Is.EqualTo(HostMetadataCache.Host.MachineName));
        }

        [Test]
        public async Task Should_have_the_original_destination_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.DestinationAddress, Is.EqualTo(InputQueueAddress));
        }

        [Test]
        public async Task Should_have_the_original_fault_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.FaultAddress, Is.EqualTo(BusAddress));
        }

        [Test]
        public async Task Should_have_the_original_response_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ResponseAddress, Is.EqualTo(BusAddress));
        }

        [Test]
        public async Task Should_have_the_original_source_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.SourceAddress, Is.EqualTo(BusAddress));
        }

        [Test]
        public async Task Should_have_the_reason()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            Assert.That(context.ReceiveContext.TransportHeaders.Get("MT-Reason", (string)null), Is.EqualTo("fault"));
        }

        [Test]
        public async Task Should_move_the_message_to_the_error_queue()
        {
            await _errorHandler;
        }

        Task<ConsumeContext<PingMessage>> _errorHandler;
        readonly Guid? _correlationId = NewId.NextGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(context =>
            {
                context.CorrelationId = _correlationId;
                context.ResponseAddress = Bus.Address;
                context.FaultAddress = Bus.Address;
            }));
        }

        protected override void ConfigureActiveMqBusHost(IActiveMqBusFactoryConfigurator configurator, IActiveMqHost host)
        {
            configurator.ReceiveEndpoint(host, "input_queue_error", x =>
            {
                x.BindMessageTopics = false;

                _errorHandler = Handled<PingMessage>(x);
            });
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context =>
            {
                throw new SerializationException("This is fine, forcing death");
            });
        }
    }
}