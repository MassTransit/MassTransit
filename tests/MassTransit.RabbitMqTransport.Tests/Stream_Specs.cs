namespace MassTransit.RabbitMqTransport.Tests;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;


[TestFixture]
public class Consuming_messages_from_a_stream
{
    [Test]
    public async Task Should_process_messages()
    {
        await using var provider = new ServiceCollection()
            .Configure<RabbitMqTransportOptions>(options => options.VHost = "test")
            .ConfigureRabbitMqTestOptions(options =>
            {
                options.CreateVirtualHostIfNotExists = true;
                options.CleanVirtualHost = true;
            })
            .AddMassTransitTestHarness(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<EventStreamConsumer, EventStreamConsumerDefinition>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var eventId = NewId.NextGuid();

        await harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>().Publish(new BusinessEvent
        {
            Id = eventId,
            Value = "Something"
        });

        Assert.That(await harness.Consumed.Any<BusinessEvent>(x => x.Context.Message.Id == eventId));
    }


    class EventStreamConsumer :
        IConsumer<BusinessEvent>
    {
        public async Task Consume(ConsumeContext<BusinessEvent> context)
        {
            await Task.Delay(1);
        }
    }


    class EventStreamConsumerDefinition :
        ConsumerDefinition<EventStreamConsumer>
    {
        public EventStreamConsumerDefinition()
        {
            EndpointName = "event-stream";
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<EventStreamConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.PrefetchCount = 100;
            endpointConfigurator.ConcurrentMessageLimit = 1;

            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rmq)
            {
                rmq.Stream("main-consumer", s =>
                {
                    s.MaxAge = TimeSpan.FromDays(14);

                    s.FromFirst();
                });
            }
        }
    }


    public record BusinessEvent
    {
        public Guid Id { get; init; }
        public string Value { get; init; }
    }
}
