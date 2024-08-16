namespace MassTransit.Metrics.Tests
{
    using MassTransit.Metrics.Tests.HarnessContracts;
    using MassTransit.Monitoring;
    using MassTransit.TestFramework.Messages;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.Metrics.Testing;
    using Microsoft.Extensions.Options;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using VerifyNUnit;
    using VerifyTests;

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
    public class Metrics_Specs
    {
        [Test]
        public async Task Should_get_metrics_with_tags()
        {
            var services = new ServiceCollection();

            services.AddOptions<InstrumentationOptions>();
            services.AddSingleton<IConfigureOptions<InstrumentationOptions>, ConfigureDefaultInstrumentationOptions>();

            await using var provider = services
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<MonitoredSubmitOrderConsumer>();
                    x.AddReceiveObserver<ReceiveObserver>();
                    x.AddSendObserver<SendObserver>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.UseNewtonsoftJsonSerializer();

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var instrumentationOptions = provider.GetRequiredService<IOptionsMonitor<InstrumentationOptions>>().CurrentValue;
            var instrumentationProperties = instrumentationOptions.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(o => o.CanRead && o.PropertyType == typeof(string))
                .Select(o => new { o.Name, Value = (string)o.GetValue(instrumentationOptions)! })
                .Where(o => o.Value != null)
                .ToList();

            var longCollectors = instrumentationProperties
                .Where(o => o.Name.EndsWith("Total") || o.Name.EndsWith("InProgress"))
                .Select(o => new MetricCollector<long>(null, InstrumentationOptions.MeterName, o.Value))
                .ToList();

            var doubleCollectors = instrumentationProperties
                .Where(o => o.Name.EndsWith("Duration"))
                .Select(o => new MetricCollector<double>(null, InstrumentationOptions.MeterName, o.Value))
                .ToList();

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<SubmitOrder> client = harness.GetRequestClient<SubmitOrder>();

            await harness.Bus.Publish(new PingMessage());

            Response<OrderSubmitted> response = await client.GetResponse<OrderSubmitted>(new
            {
                OrderId = InVar.Id,
                OrderNumber = "123"
            });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Sent.Any<OrderSubmitted>(), Is.True);
                Assert.That(await harness.Consumed.Any<SubmitOrder>(), Is.True);
            });

            List<object> snapshots =
            [
                .. longCollectors
                    .Select(o => new
                    {
                        o.Instrument!.Name,
                        LastMeasurement = o.LastMeasurement == null ? null : new { Value = (object)o.LastMeasurement.Value, o.LastMeasurement.Tags }
                    }),
                .. doubleCollectors
                    .Select(o => new
                    {
                        o.Instrument!.Name,
                        LastMeasurement = o.LastMeasurement == null ? null : new { Value = (object)"SCRUBBED", o.LastMeasurement.Tags }
                    }),
            ];

            foreach (var collector in longCollectors)
            {
                collector.Dispose();
            }

            foreach (var collector in doubleCollectors)
            {
                collector.Dispose();
            }

            var settings = new VerifySettings();
            settings.AddScrubber(scrubber =>
            {
                var destination = harness.Bus.Address.LocalPath.Trim('/');
                scrubber.Replace(destination, "BUS ADDRESS");
            });
            await Verifier.Verify(snapshots, settings);
        }

        class MonitoredSubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            public Task Consume(ConsumeContext<SubmitOrder> context)
            {
                return context.RespondAsync<OrderSubmitted>(context.Message);
            }
        }

        class ReceiveObserver : IReceiveObserver
        {
            private const string TagValue = "test";

            public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
            {
                context.AddMetricTags($"IReceiveObserver.{nameof(ConsumeFault)}", TagValue);
                return Task.CompletedTask;
            }

            public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
            {
                context.AddMetricTags($"IReceiveObserver.{nameof(PostConsume)}", TagValue);
                return Task.CompletedTask;
            }

            public Task PostReceive(ReceiveContext context)
            {
                context.AddMetricTags($"IReceiveObserver.{nameof(PostReceive)}", TagValue);
                return Task.CompletedTask;
            }

            public Task PreReceive(ReceiveContext context)
            {
                context.AddMetricTags($"IReceiveObserver.{nameof(PreReceive)}", TagValue);
                return Task.CompletedTask;
            }

            public Task ReceiveFault(ReceiveContext context, Exception exception)
            {
                context.AddMetricTags($"IReceiveObserver.{nameof(ReceiveFault)}", TagValue);
                return Task.CompletedTask;
            }
        }

        class SendObserver : ISendObserver
        {
            private const string TagValue = "test";

            public Task PostSend<T>(SendContext<T> context) where T : class
            {
                context.AddMetricTags($"ISendObserver.{nameof(PostSend)}", TagValue);
                return Task.CompletedTask;
            }

            public Task PreSend<T>(SendContext<T> context) where T : class
            {
                context.AddMetricTags($"ISendObserver.{nameof(PreSend)}", TagValue);
                return Task.CompletedTask;
            }

            public Task SendFault<T>(SendContext<T> context, Exception exception) where T : class
            {
                context.AddMetricTags($"ISendObserver.{nameof(SendFault)}", TagValue);
                return Task.CompletedTask;
            }
        }
    }
}
