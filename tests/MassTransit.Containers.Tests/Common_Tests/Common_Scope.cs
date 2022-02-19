namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    public class Resolving_a_scoped_interface_without_a_scope<TContainer> :
        InMemoryContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_resolve_the_publish_endpoint()
        {
            var publishEndpoint = GetPublishEndpoint();

            await publishEndpoint.Publish(new PingMessage());
        }

        [Test]
        public async Task Should_resolve_the_send_endpoint_provider()
        {
            var sendEndpointProvider = GetSendEndpointProvider();

            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(InputQueueAddress);

            await sendEndpoint.Send(new PingMessage());
        }

        ISendEndpointProvider GetSendEndpointProvider()
        {
            return ServiceProvider.GetRequiredService<ISendEndpointProvider>();
        }

        IPublishEndpoint GetPublishEndpoint()
        {
            return ServiceProvider.GetRequiredService<IPublishEndpoint>();
        }
    }
}
