namespace MassTransit.Tests.ContainerTests.Common_Tests;

using System;
using System.Threading.Tasks;
using DependencyInjection;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestFramework.Messages;


[TestFixture]
public class MultiBusEndpointName_Specs
{
    [Test]
    public async Task Should_allow_separate_endpoint_names_per_bus()
    {
        var otherBusBaseAddress = new Uri($"loopback://localhost/other-bus");

        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<Consumer>().Endpoint(x => x.Name = "first-bus-queue-name");
            })
            .AddMassTransit<IOtherBus>(cfg =>
            {
                cfg.AddConsumer<Consumer>()
                    .Endpoint(x => x.Name = "other-bus-queue-name");
                cfg.AddConsumer<ConsumerTwo>();

                cfg.UsingInMemory((context, configurator) =>
                {
                    configurator.Host(otherBusBaseAddress);

                    configurator.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        //await provider.GetRequiredService<IOtherBus>().Publish(new SendPingMessage());
        await harness.Bus.Publish(new PingMessage());

        IReceivedMessage<PingMessage> consumed = await harness.Consumed.SelectAsync<PingMessage>().FirstOrDefault();
        Assert.That(consumed, Is.Not.Null);
        Assert.That(consumed.Context.ReceiveContext.InputAddress, Is.EqualTo(new Uri("loopback://localhost/first-bus-queue-name")));
    }


    class Consumer :
        IConsumer<PingMessage>
    {
        public Task Consume(ConsumeContext<PingMessage> context)
        {
            return Task.CompletedTask;
        }
    }


    class ConsumerTwo :
        IConsumer<SendPingMessage>
    {
        readonly IPublishEndpoint _publishEndpoint;

        public ConsumerTwo(Bind<IBus, IPublishEndpoint> publishEndpoint)
        {
            _publishEndpoint = publishEndpoint.Value;
        }

        public async Task Consume(ConsumeContext<SendPingMessage> context)
        {
            await _publishEndpoint.Publish(new PingMessage());
        }
    }


    public record SendPingMessage;


    public interface IOtherBus :
        IBus
    {
    }
}
