namespace MassTransit.ActiveMqTransport.Tests
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using HarnessContracts;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using OpenTelemetry;
    using OpenTelemetry.Resources;
    using OpenTelemetry.Trace;
    using Testing;


    namespace HarnessContracts
    {
        using System;


        public interface SubmitOrder
        {
            Guid OrderId { get; }
            string OrderNumber { get; }
        }


        public interface OrderSubmitted
        {
            Guid OrderId { get; }
            string OrderNumber { get; }
        }
    }


    [TestFixture]
    [Explicit]
    public class OpenTelemetry_Specs
    {
        [Test]
        public async Task Should_report_telemetry_to_jaeger()
        {
            var services = new ServiceCollection();
            services.AddOpenTelemetry()
                .WithTracing(t => t.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("order-api"))
                    .AddSource(DiagnosticHeaders.DefaultListenerName));

            await using var provider = services
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<MonitoredSubmitOrderConsumer>();

                    x.UsingActiveMq((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<SubmitOrder> client = harness.GetRequestClient<SubmitOrder>();

            await client.GetResponse<OrderSubmitted>(new
            {
                OrderId = InVar.Id,
                OrderNumber = "123"
            });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Sent.Any<OrderSubmitted>(), Is.True);

                Assert.That(await harness.Consumed.Any<SubmitOrder>(), Is.True);
            });
        }


        class MonitoredSubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            public Task Consume(ConsumeContext<SubmitOrder> context)
            {
                return context.RespondAsync<OrderSubmitted>(context.Message);
            }
        }
    }
}
