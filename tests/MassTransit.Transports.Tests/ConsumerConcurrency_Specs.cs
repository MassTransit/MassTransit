namespace MassTransit.Transports.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Testing;


    public class Setting_a_concurrent_message_limit_of_one :
        TransportTest
    {
        [Test]
        public async Task Should_only_consume_one_message_at_a_time()
        {
            var orderId = NewId.NextGuid();

            await Harness.InputQueueSendEndpoint.SendBatch(new[]
            {
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = NewId.NextGuid() },
                new SubmitOrder { Id = orderId }
            });

            Assert.That(await _consumer.Consumed.Any<SubmitOrder>(x => x.Context.Message.Id == orderId), Is.True);

            await _observer.StopEndpoint();

            Assert.That(await _observer.ConcurrentDeliveryCount, Is.EqualTo(1));
        }

        ConsumerTestHarness<SubmitOrderConsumer> _consumer;
        EndpointObserver _observer;

        public Setting_a_concurrent_message_limit_of_one(Type harnessType)
            : base(harnessType)
        {
        }

        protected override Task Arrange()
        {
            _observer = new EndpointObserver(Harness);
            Harness.OnConfigureBus += cfg =>
            {
                cfg.ConnectBusObserver(new BusObserver(Harness, _observer));
            };

            Harness.OnConfigureReceiveEndpoint += x => x.ConcurrentMessageLimit = 1;

            _consumer = Harness.Consumer<SubmitOrderConsumer>();

            return Task.CompletedTask;
        }


        class BusObserver :
            IBusObserver
        {
            readonly BusTestHarness _harness;
            readonly EndpointObserver _observer;

            public BusObserver(BusTestHarness harness, EndpointObserver observer)
            {
                _harness = harness;
                _observer = observer;
            }

            public void PostCreate(IBus bus)
            {
            }

            public void CreateFaulted(Exception exception)
            {
            }

            public Task PreStart(IBus bus)
            {
                bus.ConnectReceiveEndpointObserver(_observer);
                return Task.CompletedTask;
            }

            public Task PostStart(IBus bus, Task<BusReady> busReady)
            {
                return Task.CompletedTask;
            }

            public Task StartFaulted(IBus bus, Exception exception)
            {
                return Task.CompletedTask;
            }

            public Task PreStop(IBus bus)
            {
                return Task.CompletedTask;
            }

            public Task PostStop(IBus bus)
            {
                return Task.CompletedTask;
            }

            public Task StopFaulted(IBus bus, Exception exception)
            {
                return Task.CompletedTask;
            }
        }


        class EndpointObserver :
            IReceiveEndpointObserver
        {
            readonly TaskCompletionSource<long> _concurrentDeliveryCount;
            readonly BusTestHarness _harness;
            IReceiveEndpoint _receiveEndpoint;

            public EndpointObserver(BusTestHarness harness)
            {
                _harness = harness;
                _concurrentDeliveryCount = new TaskCompletionSource<long>(TaskCreationOptions.RunContinuationsAsynchronously);
            }

            public Task<long> ConcurrentDeliveryCount => _concurrentDeliveryCount.Task;

            public Task Ready(ReceiveEndpointReady ready)
            {
                if (ready.InputAddress == _harness.InputQueueAddress)
                    _receiveEndpoint = ready.ReceiveEndpoint;

                return Task.CompletedTask;
            }

            public Task Stopping(ReceiveEndpointStopping stopping)
            {
                return Task.CompletedTask;
            }

            public Task Completed(ReceiveEndpointCompleted completed)
            {
                if (completed.InputAddress == _harness.InputQueueAddress)
                    _concurrentDeliveryCount.TrySetResult(completed.ConcurrentDeliveryCount);

                return Task.CompletedTask;
            }

            public Task Faulted(ReceiveEndpointFaulted faulted)
            {
                return Task.CompletedTask;
            }

            public async Task StopEndpoint()
            {
                if (_receiveEndpoint != null)
                    await _receiveEndpoint.Stop();
            }
        }


        class SubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            public Task Consume(ConsumeContext<SubmitOrder> context)
            {
                return Task.CompletedTask;
            }
        }


        class SubmitOrder
        {
            public Guid Id { get; set; }
        }
    }
}
