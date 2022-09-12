namespace MassTransit.Interop.NServiceBus.Tests;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serialization;
using Testing;


public class When_reading_xml_from_nsb
{
    [Test]
    public async Task Should_properly_deserialize_it()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddHandler(async (OrderPlaced message) => Console.Out.WriteLine("Order Placed: {0}, {1}", message.OrderId, message.ItemCount));

                x.AddConsumer<PlaceOrderConsumer>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseNServiceBusXmlSerializer();

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        harness.TestTimeout = TimeSpan.FromSeconds(3);

        await harness.Start();

        await harness.Bus.Publish<PlaceOrder>(new
        {
            OrderId = InVar.CorrelationId,
            OrderNumber = "123"
        }, x => ApplyStaticMessageToContext(x, ValidXml));

        Assert.IsTrue(await harness.Consumed.Any<PlaceOrder>());

        Assert.IsTrue(await harness.Published.Any<OrderPlaced>());

        Assert.IsTrue(await harness.Consumed.Any<OrderPlaced>());
    }

    void ApplyStaticMessageToContext(SendContext<PlaceOrder> context, string body)
    {
        context.Serializer = new CopyBodySerializer(NServiceBusXmlMessageSerializer.XmlContentType, new StringMessageBody(body));
    }

    const string ValidXml = @"<?xml version=""1.0""?><PlaceOrder xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://tempuri.net/Messages""><OrderId>c0147c3f-8c3d-40c9-bc2c-f71927491d01</OrderId><ItemCount xsi:nil=""true""></ItemCount></PlaceOrder>";


    public sealed class PlaceOrderConsumer : IConsumer<PlaceOrder>
    {
        readonly ILogger<PlaceOrderConsumer> _logger;

        public PlaceOrderConsumer(ILogger<PlaceOrderConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PlaceOrder> context)
        {
            _logger.LogInformation("Received PlaceOrder, OrderId = {OrderId} ({Count})", context.Message.OrderId, context.Message.ItemCount);

            var orderPlaced = new OrderPlaced { OrderId = context.Message.OrderId };

            return context.Publish(orderPlaced);
        }
    }


    public class PlaceOrder
    {
        public string OrderId { get; set; }
        public int? ItemCount { get; set; }
    }


    public class OrderPlaced
    {
        public string OrderId { get; set; }
        public int? ItemCount { get; set; }
    }
}
