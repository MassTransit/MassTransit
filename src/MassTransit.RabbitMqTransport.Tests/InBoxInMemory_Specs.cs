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
using NUnit.Framework;


namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;


    [TestFixture]
    public class Using_scheduled_redelivery_for_a_specific_message_type :
        RabbitMqTestFixture
    {
        Task<ConsumeContext<Fault<TestCommand>>> _faulted;

        [Test]
        public async Task Should_not_send_twice()
        {
            await Bus.Publish<TestCommand>(new {Id = NewId.NextGuid()});

            await _faulted;

            Assert.That(TestHandler.Count, Is.EqualTo(0));
        }

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            configurator.ReceiveEndpoint(host, "input-fault", endpointConfigurator =>
            {
                _faulted = Handled<Fault<TestCommand>>(endpointConfigurator);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<TestHandler>(x =>
            {
                x.Message<TestCommand>(m =>
                {
                    m.UseDelayedRedelivery(r => r.Interval(1, TimeSpan.FromMilliseconds(100)));
                    m.UseRetry(r => r.Interval(1, TimeSpan.FromMilliseconds(100)));
                    m.UseInMemoryOutbox();
                });
            });
        }


        public interface TestCommand
        {
            Guid Id { get; }
        }


        public interface InnerCommand
        {
            Guid Id { get; }
        }


        public class TestHandler :
            IConsumer<TestCommand>,
            IConsumer<InnerCommand>
        {
            public static int Count = 0;

            public Task Consume(ConsumeContext<TestCommand> context)
            {
                context.Publish<InnerCommand>(new {Id = context.Message.Id});

                throw new Exception("something went wrong...");
            }

            public Task Consume(ConsumeContext<InnerCommand> context)
            {
                ++Count;

                return TaskUtil.Completed;
            }
        }
    }


    [TestFixture]
    public class Using_scheduled_redelivery_for_a_specific_message_type_with_send :
        RabbitMqTestFixture
    {
        Task<ConsumeContext<Fault<TestCommand>>> _faulted;

        [Test]
        public async Task Should_not_send_twice()
        {
            await Bus.Publish<TestCommand>(new {Id = NewId.NextGuid()});

            await _faulted;

            Assert.That(TestHandler.Count, Is.EqualTo(0));
        }

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            configurator.ReceiveEndpoint(host, "input-fault", endpointConfigurator =>
            {
                _faulted = Handled<Fault<TestCommand>>(endpointConfigurator);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<TestHandler>(x =>
            {
                x.Message<TestCommand>(m =>
                {
                    m.UseDelayedRedelivery(r => r.Interval(1, TimeSpan.FromMilliseconds(100)));
                    m.UseRetry(r => r.Interval(1, TimeSpan.FromMilliseconds(100)));
                    m.UseInMemoryOutbox();
                });
            });
        }


        public interface TestCommand
        {
            Guid Id { get; }
        }


        public interface InnerCommand
        {
            Guid Id { get; }
        }


        public class TestHandler :
            IConsumer<TestCommand>,
            IConsumer<InnerCommand>
        {
            public static int Count = 0;

            public Task Consume(ConsumeContext<TestCommand> context)
            {
                context.Send<InnerCommand>(context.ReceiveContext.InputAddress, new {Id = context.Message.Id});

                throw new Exception("something went wrong...");
            }

            public Task Consume(ConsumeContext<InnerCommand> context)
            {
                ++Count;

                return TaskUtil.Completed;
            }
        }
    }


    [TestFixture]
    public class Using_scheduled_redelivery_for_all_message_types :
        RabbitMqTestFixture
    {
        Task<ConsumeContext<Fault<TestCommand>>> _faulted;

        [Test]
        public async Task Should_not_send_twice()
        {
            await Bus.Publish<TestCommand>(new {Id = NewId.NextGuid()});

            await _faulted;

            Assert.That(TestHandler.Count, Is.EqualTo(0));
        }

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            configurator.ReceiveEndpoint(host, "input-fault", endpointConfigurator =>
            {
                _faulted = Handled<Fault<TestCommand>>(endpointConfigurator);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.UseDelayedRedelivery(r => r.Interval(1, TimeSpan.FromMilliseconds(100)));
            configurator.UseRetry(r => r.Interval(1, TimeSpan.FromMilliseconds(100)));
            configurator.UseInMemoryOutbox();

            configurator.Consumer<TestHandler>();
        }


        public interface TestCommand
        {
            Guid Id { get; }
        }


        public interface InnerCommand
        {
            Guid Id { get; }
        }


        public class TestHandler :
            IConsumer<TestCommand>,
            IConsumer<InnerCommand>
        {
            public static int Count = 0;

            public Task Consume(ConsumeContext<TestCommand> context)
            {
                context.Publish<InnerCommand>(new {context.Message.Id});

                throw new Exception("something went wrong...");
            }

            public Task Consume(ConsumeContext<InnerCommand> context)
            {
                ++Count;

                return TaskUtil.Completed;
            }
        }
    }
}
