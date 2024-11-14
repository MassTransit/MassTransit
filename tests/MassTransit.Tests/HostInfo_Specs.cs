namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using Metadata;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Host_info_should_be_included_on_json_serialization :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_match_the_sending_host_information()
        {
            await Bus.Publish(new PingMessage());

            ConsumeContext<PingMessage> context = await _handled;

            Assert.That(context.Host, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(context.Host.MachineName, Is.EqualTo(HostMetadataCache.Host.MachineName));
                Assert.That(context.Host.Assembly, Is.EqualTo(HostMetadataCache.Host.Assembly));
                Assert.That(context.Host.AssemblyVersion, Is.EqualTo(HostMetadataCache.Host.AssemblyVersion));
                Assert.That(context.Host.FrameworkVersion, Is.EqualTo(HostMetadataCache.Host.FrameworkVersion));
                Assert.That(context.Host.MassTransitVersion, Is.EqualTo(HostMetadataCache.Host.MassTransitVersion));
                Assert.That(context.Host.OperatingSystemVersion, Is.EqualTo(HostMetadataCache.Host.OperatingSystemVersion));
                Assert.That(context.Host.ProcessName, Is.EqualTo(HostMetadataCache.Host.ProcessName));
                Assert.That(context.Host.ProcessId, Is.EqualTo(HostMetadataCache.Host.ProcessId));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<PingMessage>(configurator);
        }
    }
}
