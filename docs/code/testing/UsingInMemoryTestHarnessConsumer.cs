namespace UsingInMemoryTestHarnessConsumer;

using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using NUnit.Framework;

[TestFixture]
public class Submitting_an_order
{
    [Test]
    public async Task Should_publish_the_order_submitted_event()
    {
        var harness = new InMemoryTestHarness();
        var consumerHarness = harness.Consumer<SubmitOrderConsumer>();

        await harness.Start();
        try
        {
            await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
            {
                OrderId = InVar.Id
            });

            // did the endpoint consume the message
            Assert.That(await harness.Consumed.Any<SubmitOrder>());

            // did the actual consumer consume the message
            Assert.That(await consumerHarness.Consumed.Any<SubmitOrder>());

            // the consumer publish the event
            Assert.That(await harness.Published.Any<OrderSubmitted>());

            // ensure that no faults were published by the consumer
            Assert.That(await harness.Published.Any<Fault<SubmitOrder>>(), Is.False);
        }
        finally
        {
            await harness.Stop();
        }
    }
}

public record SubmitOrder
{
    public Guid OrderId { get; init; }
}

public record OrderSubmitted
{
    public Guid OrderId { get; init; }
}

class SubmitOrderConsumer :
    IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        await context.Publish<OrderSubmitted>(new
        {
            context.Message.OrderId
        });
    }
}
