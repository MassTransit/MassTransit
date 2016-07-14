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
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using Shouldly;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class A_serialization_exception :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_have_the_correlation_id()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.CorrelationId.ShouldBe(_correlationId);
        }

        [Test]
        public async Task Should_have_the_exception()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.ReceiveContext.TransportHeaders.Get("MT-Fault-Message", (string)null).ShouldBe("This is fine, forcing death");
        }

        [Test]
        public async Task Should_have_the_host_machine_name()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.ReceiveContext.TransportHeaders.Get("MT-Host-MachineName", (string)null).ShouldBe(HostMetadataCache.Host.MachineName);
        }

        [Test]
        public async Task Should_have_the_original_destination_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.DestinationAddress.ShouldBe(InputQueueAddress);
        }

        [Test]
        public async Task Should_have_the_original_fault_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.FaultAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_have_the_original_response_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.ResponseAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_have_the_original_source_address()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.SourceAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_have_the_reason()
        {
            ConsumeContext<PingMessage> context = await _errorHandler;

            context.ReceiveContext.TransportHeaders.Get("MT-Reason", (string)null).ShouldBe("fault");
        }

        [Test]
        public async Task Should_move_the_message_to_the_error_queue()
        {
            await _errorHandler;
        }

        Task<ConsumeContext<PingMessage>> _errorHandler;
        readonly Guid? _correlationId = NewId.NextGuid();

        [OneTimeSetUp]
        public void Setup()
        {
            Await(() => InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext<PingMessage>>(context =>
            {
                context.CorrelationId = _correlationId;
                context.ResponseAddress = Bus.Address;
                context.FaultAddress = Bus.Address;
            })));
        }

        protected override void ConfigureBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            configurator.ReceiveEndpoint(host, "input_queue_error", x =>
            {
                x.PurgeOnStartup = true;

                _errorHandler = Handled<PingMessage>(x);
            });
        }

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context =>
            {
                throw new SerializationException("This is fine, forcing death");
            });
        }
    }


    [TestFixture]
    public class A_serialization_exception_from_a_bad_messagee :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_have_the_invalid_body()
        {
            _body.ShouldBe("[]");
        }

        [Test]
        public async Task Should_have_the_host_machine_name()
        {
            var header = Encoding.UTF8.GetString((byte[])_basicGetResult.BasicProperties.Headers["MT-Host-MachineName"]);
            header.ShouldBe(HostMetadataCache.Host.MachineName);
        }

        [Test]
        public async Task Should_have_the_reason()
        {
            var header = Encoding.UTF8.GetString((byte[])_basicGetResult.BasicProperties.Headers["MT-Reason"]);

            header.ShouldBe("fault");
        }

        IRabbitMqHost _host;
        string _body;
        BasicGetResult _basicGetResult;

        [OneTimeSetUp]
        public void Setup()
        {
            TaskUtil.Await(async () =>
            {
                var connectionFactory = _host.Settings.GetConnectionFactory();
                using (var connection = connectionFactory.CreateConnection())
                using (var model = connection.CreateModel())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes("[]");

                    model.BasicPublish("input_queue", "", model.CreateBasicProperties(), bytes);

                    await Task.Delay(3000).ConfigureAwait(false);

                    _basicGetResult = model.BasicGet("input_queue_error", true);

                    _body = Encoding.UTF8.GetString(_basicGetResult.Body);

                    model.Close(200, "Cleanup complete");
                    connection.Close(200, "Cleanup complete");
                }
            });
        }

        protected override void ConfigureBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            _host = host;
        }

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handled<PingMessage>(configurator);
        }
    }
}