namespace MassTransit.AmazonSqsTransport.Tests
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
                    .AddSource(DiagnosticHeaders.DefaultListenerName)
                    .AddJaegerExporter(o =>
                    {
                        o.AgentHost = "localhost";
                        o.AgentPort = 6831;

                        // Examples for the rest of the options, defaults unless otherwise specified
                        // Omitting Process Tags example as Resource API is recommended for additional tags
                        o.MaxPayloadSizeInBytes = 4096;

                        // Using Batch Exporter (which is default)
                        // The other option is ExportProcessorType.Simple
                        o.ExportProcessorType = ExportProcessorType.Batch;
                        o.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>
                        {
                            MaxQueueSize = 2048,
                            ScheduledDelayMilliseconds = 5000,
                            ExporterTimeoutMilliseconds = 30000,
                            MaxExportBatchSize = 512
                        };
                    })
                    .Build());

            await using var provider = services
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<MonitoredSubmitOrderConsumer>();

                    x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.LocalstackHost();

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
