﻿namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture(ActiveMqHostAddress.ActiveMqScheme)]
    [TestFixture(ActiveMqHostAddress.AmqpScheme)]
    [Category("Flaky")]
    public class Using_delayed_redelivery_for_a_specific_message_type :
        ActiveMqTestFixture
    {
        public Using_delayed_redelivery_for_a_specific_message_type(string protocol)
            : base(protocol)
        {
        }

        [Test]
        public async Task Should_not_send_twice()
        {
            await Bus.Publish<TestCommand>(new {Id = NewId.NextGuid()});

            await _faulted;

            Assert.That(TestHandler.Count, Is.EqualTo(0));
        }

        Task<ConsumeContext<Fault<TestCommand>>> _faulted;

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input-fault", endpointConfigurator =>
            {
                _faulted = Handled<Fault<TestCommand>>(endpointConfigurator);
            });
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
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
            public static int Count;

            public Task Consume(ConsumeContext<InnerCommand> context)
            {
                ++Count;

                return Task.CompletedTask;
            }

            public Task Consume(ConsumeContext<TestCommand> context)
            {
                context.Publish<InnerCommand>(new {context.Message.Id});

                throw new Exception("something went wrong...");
            }
        }
    }


    [TestFixture(ActiveMqHostAddress.ActiveMqScheme)]
    [TestFixture(ActiveMqHostAddress.AmqpScheme)]
    [Category("Flaky")]
    public class Using_delayed_redelivery_for_a_specific_message_type_with_send :
        ActiveMqTestFixture
    {
        public Using_delayed_redelivery_for_a_specific_message_type_with_send(string protocol)
            : base(protocol)
        {
        }

        [Test]
        public async Task Should_not_send_twice()
        {
            await Bus.Publish<TestCommand>(new {Id = NewId.NextGuid()});

            await _faulted;

            Assert.That(TestHandler.Count, Is.EqualTo(0));
        }

        Task<ConsumeContext<Fault<TestCommand>>> _faulted;

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input-fault", endpointConfigurator =>
            {
                _faulted = Handled<Fault<TestCommand>>(endpointConfigurator);
            });
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
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
            public static int Count;

            public Task Consume(ConsumeContext<InnerCommand> context)
            {
                ++Count;

                return Task.CompletedTask;
            }

            public Task Consume(ConsumeContext<TestCommand> context)
            {
                context.Send<InnerCommand>(context.ReceiveContext.InputAddress, new {context.Message.Id});

                throw new Exception("something went wrong...");
            }
        }
    }


    [TestFixture(ActiveMqHostAddress.ActiveMqScheme)]
    [TestFixture(ActiveMqHostAddress.AmqpScheme)]
    [Category("Flaky")]
    public class Using_delayed_redelivery_for_all_message_types :
        ActiveMqTestFixture
    {
        public Using_delayed_redelivery_for_all_message_types(string protocol)
            : base(protocol)
        {
        }

        [Test]
        public async Task Should_not_send_twice()
        {
            await Bus.Publish<TestCommand>(new {Id = NewId.NextGuid()});

            await _faulted;

            Assert.That(TestHandler.Count, Is.EqualTo(0));
        }

        Task<ConsumeContext<Fault<TestCommand>>> _faulted;

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input-fault", endpointConfigurator =>
            {
                _faulted = Handled<Fault<TestCommand>>(endpointConfigurator);
            });
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.UseDelayedRedelivery(r => r.Interval(1, TimeSpan.FromMilliseconds(100)));
            configurator.UseMessageRetry(r => r.Interval(1, TimeSpan.FromMilliseconds(100)));
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
            public static int Count;

            public Task Consume(ConsumeContext<InnerCommand> context)
            {
                ++Count;

                return Task.CompletedTask;
            }

            public Task Consume(ConsumeContext<TestCommand> context)
            {
                context.Publish<InnerCommand>(new {context.Message.Id});

                throw new Exception("something went wrong...");
            }
        }
    }
}
