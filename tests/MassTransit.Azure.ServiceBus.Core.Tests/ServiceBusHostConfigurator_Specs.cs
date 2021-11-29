namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System.Threading.Tasks;
    using MassTransit.Configuration;
    using NUnit.Framework;


    [TestFixture]
    public class Service_bus_host_settings_from_connection_string
    {
        [Test]
        public async Task Should_succeed_with_simple_endpoint()
        {
            var connectionString = "Endpoint=sb://my-endpoint.servicebus.windows.net;SharedAccessKeyName=SomeKeyName;SharedAccessKey=SomeKey";
            var configurator = new ServiceBusHostConfigurator(connectionString);

            Assert.That(configurator.Settings.ServiceUri.ToString(), Is.EqualTo("sb://my-endpoint.servicebus.windows.net/"));
        }

        [Test]
        public async Task Should_succeed_with_endpoint_at_the_end_without_semicolon()
        {
            var connectionString = "SharedAccessKeyName=SomeKeyName;SharedAccessKey=SomeKey;Endpoint=sb://my-endpoint.servicebus.windows.net";
            var configurator = new ServiceBusHostConfigurator(connectionString);

            Assert.That(configurator.Settings.ServiceUri.ToString(), Is.EqualTo("sb://my-endpoint.servicebus.windows.net/"));
        }

        [Test]
        public async Task Endpoint_should_include_namespace()
        {
            var connectionString = "Endpoint=sb://my-endpoint.servicebus.windows.net/someNamespace;SharedAccessKeyName=SomeKeyName;SharedAccessKey=SomeKey";
            var configurator = new ServiceBusHostConfigurator(connectionString);

            Assert.That(configurator.Settings.ServiceUri.ToString(), Is.EqualTo("sb://my-endpoint.servicebus.windows.net/someNamespace"));
        }
    }
}
