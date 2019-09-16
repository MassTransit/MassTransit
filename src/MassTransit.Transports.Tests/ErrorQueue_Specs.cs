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
    namespace ErrorHandling
    {
        using System;
        using System.Runtime.Serialization;
        using System.Threading.Tasks;
        using AmazonSqsTransport.Testing;
        using Metadata;
        using NUnit.Framework;
        using Testing;


        public interface FailingCommand
        {
            Guid CorrelationId { get; }
        }


        public class A_serialization_exception :
            TransportTest
        {
            [Test]
            public async Task Should_have_the_correlation_id()
            {
                ConsumeContext<FailingCommand> context = await _errorHandler;

                Assert.That(context.CorrelationId, Is.EqualTo(_correlationId));
            }

            [Test]
            public async Task Should_have_the_exception()
            {
                ConsumeContext<FailingCommand> context = await _errorHandler;

                Assert.That(context.ReceiveContext.TransportHeaders.Get("MT-Fault-Message", default(string)), Is.EqualTo("This is fine, forcing death"));
            }

            [Test]
            public async Task Should_have_the_host_machine_name()
            {
                if (HarnessType == typeof(AmazonSqsTestHarness))
                    Assert.Ignore("Amazon SQS does not support enough message attributes to include host data");

                ConsumeContext<FailingCommand> context = await _errorHandler;

                Assert.That(context.ReceiveContext.TransportHeaders.Get("MT-Host-MachineName", default(string)),
                    Is.EqualTo(HostMetadataCache.Host.MachineName));
            }

            [Test]
            public async Task Should_have_the_reason()
            {
                ConsumeContext<FailingCommand> context = await _errorHandler;

                Assert.That(context.ReceiveContext.TransportHeaders.Get(MessageHeaders.Reason, default(string)), Is.EqualTo("fault"));
            }

            [Test]
            public async Task Should_have_the_original_destination_address()
            {
                ConsumeContext<FailingCommand> context = await _errorHandler;

                Assert.That(context.DestinationAddress, Is.EqualTo(Harness.InputQueueAddress));
            }

            [Test]
            public async Task Should_have_the_original_fault_address()
            {
                ConsumeContext<FailingCommand> context = await _errorHandler;

                Assert.That(context.FaultAddress, Is.EqualTo(Harness.BusAddress));
            }

            [Test]
            public async Task Should_have_the_original_response_address()
            {
                ConsumeContext<FailingCommand> context = await _errorHandler;

                Assert.That(context.ResponseAddress, Is.EqualTo(Harness.BusAddress));
            }

            [Test]
            public async Task Should_have_the_original_source_address()
            {
                ConsumeContext<FailingCommand> context = await _errorHandler;

                Assert.That(context.SourceAddress, Is.EqualTo(Harness.BusAddress));
            }

            [Test]
            public async Task Should_move_the_message_to_the_error_queue()
            {
                await _errorHandler;
            }

            [Test]
            public async Task Should_publish_the_fault()
            {
                await _faultHandler;
            }

            [Test]
            public async Task Should_have_the_correlation_id_in_fault()
            {
                ConsumeContext<Fault<FailingCommand>> context = await _faultHandler;

                Assert.That(context.CorrelationId, Is.EqualTo(_correlationId));
            }

            [Test]
            public async Task Should_have_the_exception_in_fault()
            {
                ConsumeContext<Fault<FailingCommand>> context = await _faultHandler;

                Assert.That(context.Message.Exceptions[0].Message, Is.EqualTo("This is fine, forcing death"));
            }

            [Test]
            public async Task Should_have_the_host_machine_name_in_fault()
            {
                if (HarnessType == typeof(AmazonSqsTestHarness))
                    Assert.Ignore("Amazon SQS does not support enough message attributes to include host data");

                ConsumeContext<Fault<FailingCommand>> context = await _faultHandler;

                Assert.That(context.Host.MachineName, Is.EqualTo(HostMetadataCache.Host.MachineName));
            }

            [Test]
            public async Task Should_have_a_null_fault_address_in_fault()
            {
                ConsumeContext<Fault<FailingCommand>> context = await _faultHandler;

                Assert.That(context.FaultAddress, Is.Null);
            }

            [Test]
            public async Task Should_have_a_null_response_address_in_fault()
            {
                ConsumeContext<Fault<FailingCommand>> context = await _faultHandler;

                Assert.That(context.ResponseAddress, Is.Null);
            }

            [Test]
            public async Task Should_have_the_original_source_address_in_fault()
            {
                ConsumeContext<Fault<FailingCommand>> context = await _faultHandler;

                Assert.That(context.SourceAddress, Is.EqualTo(Harness.InputQueueAddress));
            }

            Task<ConsumeContext<FailingCommand>> _errorHandler;
            readonly Guid? _correlationId = NewId.NextGuid();
            ConsumerTestHarness<TestCommandConsumer> _consumer;
            Task<ConsumeContext<Fault<FailingCommand>>> _faultHandler;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _consumer = Harness.Consumer<TestCommandConsumer>();

                Harness.OnConfigureBus += cfg =>
                {
                    cfg.ReceiveEndpoint(Harness.InputQueueName + "_error", x =>
                    {
                        _errorHandler = Harness.Handled<FailingCommand>(x);
                    });
                };

                await Harness.Start();

                _faultHandler = Harness.SubscribeHandler<Fault<FailingCommand>>();

                await Harness.InputQueueSendEndpoint.Send<FailingCommand>(new {CorrelationId = NewId.NextGuid()}, context =>
                {
                    context.CorrelationId = _correlationId;
                    context.ResponseAddress = Harness.BusAddress;
                    context.FaultAddress = Harness.BusAddress;
                });
            }

            [OneTimeTearDown]
            public Task Stop()
            {
                return Harness.Stop();
            }


            class TestCommandConsumer :
                IConsumer<FailingCommand>
            {
                public Task Consume(ConsumeContext<FailingCommand> context)
                {
                    throw new SerializationException("This is fine, forcing death");
                }
            }


            public A_serialization_exception(Type harnessType)
                : base(harnessType)
            {
                Subscribe = false;
            }
        }
    }
}
