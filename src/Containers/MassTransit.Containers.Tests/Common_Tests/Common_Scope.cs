namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    public abstract class Common_Scope :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_resolve_the_send_endpoint_provider()
        {
            var sendEndpointProvider = GetSendEndpointProvider();

            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(InputQueueAddress);

            await sendEndpoint.Send(new PingMessage());
        }

        [Test]
        public async Task Should_resolve_the_publish_endpoint()
        {
            var publishEndpoint = GetPublishEndpoint();

            await publishEndpoint.Publish(new PingMessage());
        }

        protected abstract ISendEndpointProvider GetSendEndpointProvider();
        protected abstract IPublishEndpoint GetPublishEndpoint();
    }
}
