namespace MassTransit.PrometheusIntegration.Tests
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Prometheus;
    using TestFramework;


    [TestFixture]
    public class BusInstanceCount_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_capture_the_bus_instance_metric()
        {
            await using var stream = new MemoryStream();
            await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);

            var text = Encoding.UTF8.GetString(stream.ToArray());

            Assert.That(text.Contains("mt_bus{service_name=\"unit_test\"} 1"));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UsePrometheusMetrics(serviceName: "unit_test");
        }
    }
}
