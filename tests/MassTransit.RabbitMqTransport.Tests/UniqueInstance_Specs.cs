namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using MassTransit.Configuration;
    using MassTransit.Testing;
    using NUnit.Framework;


    [TestFixture]
    public class Specifying_a_unique_instance_for_an_endpoint
    {
        [Test]
        public async Task Should_fan_out_published_messages()
        {
            var endpointSettings = new EndpointSettings<IEndpointDefinition<EventConsumer>> { InstanceId = "27" };
            var endpointDefinition = new ConsumerEndpointDefinition<EventConsumer>(endpointSettings);

            var firstHarness = new RabbitMqTestHarness();
            var firstConsumer = new EventConsumer(firstHarness.GetTask<ConsumeContext<SomeEvent>>());
            firstHarness.OnConfigureRabbitMqBus += configurator =>
            {
                configurator.ReceiveEndpoint(endpointDefinition, KebabCaseEndpointNameFormatter.Instance, e =>
                {
                    e.Consumer(() => firstConsumer);
                });
            };

            await firstHarness.Start();
            try
            {
                endpointSettings = new EndpointSettings<IEndpointDefinition<EventConsumer>> { InstanceId = "42" };
                endpointDefinition = new ConsumerEndpointDefinition<EventConsumer>(endpointSettings);

                var secondHarness = new RabbitMqTestHarness();
                var secondConsumer = new EventConsumer(secondHarness.GetTask<ConsumeContext<SomeEvent>>());
                secondHarness.OnConfigureRabbitMqBus += configurator =>
                {
                    configurator.ReceiveEndpoint(endpointDefinition, KebabCaseEndpointNameFormatter.Instance, e =>
                    {
                        e.Consumer(() => secondConsumer);
                    });
                };

                await secondHarness.Start();
                try
                {
                    await firstHarness.Bus.Publish(new SomeEvent());

                    await firstConsumer.Completed;

                    await secondConsumer.Completed;
                }
                finally
                {
                    await secondHarness.Stop();
                }
            }
            finally
            {
                await firstHarness.Stop();
            }
        }


        class EventConsumer
            : IConsumer<SomeEvent>
        {
            readonly TaskCompletionSource<ConsumeContext<SomeEvent>> _source;

            public EventConsumer(TaskCompletionSource<ConsumeContext<SomeEvent>> source)
            {
                _source = source;
            }

            public Task<ConsumeContext<SomeEvent>> Completed => _source.Task;

            public Task Consume(ConsumeContext<SomeEvent> context)
            {
                _source.TrySetResult(context);

                return Task.CompletedTask;
            }
        }
    }


    public class SomeEvent
    {
    }
}
