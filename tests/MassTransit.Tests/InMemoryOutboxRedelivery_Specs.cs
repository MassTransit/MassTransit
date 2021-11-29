namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    [Category("Flaky")]
    public class Using_delayed_redelivery_for_a_specific_message_type :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_send_twice()
        {
            await Bus.Publish<TestCommand>(new {Id = NewId.NextGuid()});

            await _faulted;

            Assert.That(TestHandler.Count, Is.EqualTo(0));
        }

        Task<ConsumeContext<Fault<TestCommand>>> _faulted;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input-fault", endpointConfigurator =>
            {
                _faulted = Handled<Fault<TestCommand>>(endpointConfigurator);
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
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


    [TestFixture]
    [Category("Flaky")]
    public class Using_delayed_redelivery_for_a_specific_message_type_with_send :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_send_twice()
        {
            await Bus.Publish<TestCommand>(new {Id = NewId.NextGuid()});

            await _faulted;

            Assert.That(TestHandler.Count, Is.EqualTo(0));
        }

        Task<ConsumeContext<Fault<TestCommand>>> _faulted;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input-fault", endpointConfigurator =>
            {
                _faulted = Handled<Fault<TestCommand>>(endpointConfigurator);
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
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


    [TestFixture]
    [Category("Flaky")]
    public class Using_delayed_redelivery_for_all_message_types :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_send_twice()
        {
            await Bus.Publish<TestCommand>(new {Id = NewId.NextGuid()});

            await _faulted;

            Assert.That(TestHandler.Count, Is.EqualTo(0));
        }

        Task<ConsumeContext<Fault<TestCommand>>> _faulted;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input-fault", endpointConfigurator =>
            {
                _faulted = Handled<Fault<TestCommand>>(endpointConfigurator);
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
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
