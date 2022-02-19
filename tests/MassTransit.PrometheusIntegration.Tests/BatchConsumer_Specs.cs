namespace MassTransit.PrometheusIntegration.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Prometheus;
    using TestFramework;


    [TestFixture]
    public class BatchConsumer_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_capture_the_bus_instance_metric()
        {
            await InputQueueSendEndpoint.Send(new BatchMessage());
            await InputQueueSendEndpoint.Send(new BatchMessage());
            await InputQueueSendEndpoint.Send(new BatchMessage());
            await InputQueueSendEndpoint.Send(new BatchMessage());
            await InputQueueSendEndpoint.Send(new BatchMessage());

            await Bus.Publish(new BatchMessage());
            await Bus.Publish(new BatchMessage());
            await Bus.Publish(new BatchMessage());

            await InactivityTask;

            await using var stream = new MemoryStream();
            await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);

            var text = Encoding.UTF8.GetString(stream.ToArray());

            Console.WriteLine(text);

            Assert.That(text.Contains("mt_publish_total{service_name=\"unit_test\",message_type=\"BatchMessage\"} 3"), "publish");
            Assert.That(text.Contains("mt_send_total{service_name=\"unit_test\",message_type=\"BatchMessage\"} 5"), "send");
            Assert.That(text.Contains("mt_receive_total{service_name=\"unit_test\",endpoint_address=\"input_queue\"} 8"), "receive");
            Assert.That(
                text.Contains("mt_consume_total{service_name=\"unit_test\",message_type=\"BatchMessage\",consumer_type=\"BatchConsumer_BatchMessage\"} 8"),
                "batch");
            Assert.That(text.Contains("mt_consume_total{service_name=\"unit_test\",message_type=\"Batch_BatchMessage\",consumer_type=\"TestBatchConsumer\"} 1"),
                "consume");
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UsePrometheusMetrics(serviceName: "unit_test");
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConcurrentMessageLimit = 10;
            configurator.Consumer(() => new TestBatchConsumer(), x =>
                x.Options<BatchOptions>(options => options.SetMessageLimit(8).SetTimeLimit(s: 5)));
        }


        public class BatchMessage
        {
        }


        public class TestBatchConsumer :
            IConsumer<Batch<BatchMessage>>
        {
            public Task Consume(ConsumeContext<Batch<BatchMessage>> context)
            {
                return Task.CompletedTask;
            }
        }
    }


    [TestFixture]
    public class BatchConsumer_ConnectEndpoint :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_capture_the_bus_instance_metric()
        {
            var receiveEndpoint = Bus.ConnectReceiveEndpoint("batching", configurator =>
            {
                if (configurator is IInMemoryReceiveEndpointConfigurator cfg)
                    cfg.ConcurrentMessageLimit = 10;

                configurator.Consumer(() => new TestBatchConsumer(), x =>
                    x.Options<BatchOptions>(options => options.SetMessageLimit(8).SetTimeLimit(s: 5)));
            });

            await receiveEndpoint.Ready;

            await Bus.Publish(new BatchMessage());
            await Bus.Publish(new BatchMessage());
            await Bus.Publish(new BatchMessage());
            await Bus.Publish(new BatchMessage());

            await Bus.Publish(new BatchMessage());
            await Bus.Publish(new BatchMessage());
            await Bus.Publish(new BatchMessage());
            await Bus.Publish(new BatchMessage());

            await InactivityTask;

            await using var stream = new MemoryStream();
            await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);

            var text = Encoding.UTF8.GetString(stream.ToArray());

            Console.WriteLine(text);

            Assert.That(text.Contains("mt_publish_total{service_name=\"unit_test\",message_type=\"BatchMessage\"} 8"), "publish");
            Assert.That(text.Contains("mt_receive_total{service_name=\"unit_test\",endpoint_address=\"batching\"} 8"), "receive");
            Assert.That(
                text.Contains("mt_consume_total{service_name=\"unit_test\",message_type=\"BatchMessage\",consumer_type=\"BatchConsumer_BatchMessage\"} 8"),
                "batch");
            Assert.That(text.Contains("mt_consume_total{service_name=\"unit_test\",message_type=\"Batch_BatchMessage\",consumer_type=\"TestBatchConsumer\"} 1"),
                "consume");
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UsePrometheusMetrics(serviceName: "unit_test");
        }


        public class BatchMessage
        {
        }


        public class TestBatchConsumer :
            IConsumer<Batch<BatchMessage>>
        {
            public Task Consume(ConsumeContext<Batch<BatchMessage>> context)
            {
                return Task.CompletedTask;
            }
        }
    }
}
