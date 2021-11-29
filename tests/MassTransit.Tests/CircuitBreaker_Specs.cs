namespace MassTransit.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using MassTransit.Testing.Implementations;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Using_the_circuit_breaker :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_work()
        {
            for (var i = 1; i <= 30; i++)
            {
                await Bus.Publish(new BreakingMessage { MessageIndex = i });
                await Task.Delay(50);
            }

            await _activityMonitor.AwaitBusInactivity();

            // this is broken, because the faults aren't produced by an open circuit breaker
            Assert.That(_faultCount, Is.GreaterThanOrEqualTo(3));
        }

        int _faultCount;
        IBusActivityMonitor _activityMonitor;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.UseCircuitBreaker(x =>
            {
                x.ActiveThreshold = 5;
                x.ResetInterval = TimeSpan.FromSeconds(15);
                x.TrackingPeriod = TimeSpan.FromSeconds(10);
                x.TripThreshold = 20;
            });

            configurator.Consumer(() => new BreakingConsumer());
        }

        protected override void ConnectObservers(IBus bus)
        {
            _activityMonitor = bus.CreateBusActivityMonitor(TimeSpan.FromMilliseconds(500));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("faults", cfg =>
            {
                cfg.Handler<Fault<BreakingMessage>>(context =>
                {
                    Interlocked.Increment(ref _faultCount);

                    return Task.CompletedTask;
                });
            });
        }


        public class BreakingMessage
        {
            public int MessageIndex { get; set; }
        }


        class BreakingConsumer :
            IConsumer<BreakingMessage>
        {
            public Task Consume(ConsumeContext<BreakingMessage> context)
            {
                if (context.Message.MessageIndex % 2 == 0)
                    throw new IntentionalTestException("Every other message seems to fail");

                return Task.CompletedTask;
            }
        }
    }
}
