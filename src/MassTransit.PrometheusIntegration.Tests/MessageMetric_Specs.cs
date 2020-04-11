namespace MassTransit.PrometheusIntegration.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Prometheus;
    using TestFramework;
    using TestFramework.Messages;
    using Testing;
    using Testing.Indicators;


    [TestFixture]
    public class MessageMetric_Specs :
        InMemoryTestFixture
    {
        IBusActivityMonitor _activityMonitor;

        [Test]
        public async Task Should_capture_the_bus_instance_metric()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            await Bus.Publish(new PingMessage());
            await Bus.Publish(new PingMessage());
            await Bus.Publish(new PingMessage());

            await _activityMonitor.AwaitBusInactivity(TestCancellationToken);

            using var stream = new MemoryStream();
            await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);

            var text = Encoding.UTF8.GetString(stream.ToArray());

            Assert.That(text.Contains("mt_message_publish_total{service_name=\"unit_test\",message_type=\"PingMessage\"} 3"));
            Assert.That(text.Contains("mt_message_send_total{service_name=\"unit_test\",message_type=\"PingMessage\"} 5"));
            Assert.That(text.Contains("mt_receive_total{service_name=\"unit_test\",endpoint=\"input_queue\"} 8"));
            Assert.That(text.Contains("mt_message_consume_total{service_name=\"unit_test\",message_type=\"PingMessage\",consumer_type=\"TestConsumer\"} 8"));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UsePrometheusMetrics(serviceName: "unit_test");
        }

        protected override void ConnectObservers(IBus bus)
        {
            _activityMonitor = bus.CreateBusActivityMonitor(TimeSpan.FromMilliseconds(500));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new TestConsumer());
        }
    }


    public class TestConsumer :
        IConsumer<PingMessage>
    {
        public Task Consume(ConsumeContext<PingMessage> context)
        {
            return Task.CompletedTask;
        }
    }
}
