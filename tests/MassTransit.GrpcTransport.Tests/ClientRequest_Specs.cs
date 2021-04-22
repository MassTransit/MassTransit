namespace MassTransit.GrpcTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_request_from_the_client_bus :
        GrpcClientTestFixture
    {
        public Sending_a_request_from_the_client_bus()
        {
            TestTimeout = TimeSpan.FromSeconds(5);
        }

        [Test]
        public async Task Should_receive_the_response()
        {
            IRequestClient<PingMessage> client = CreateRequestClient<PingMessage>();

            Task<Response<PongMessage>> response = client.GetResponse<PongMessage>(new PingMessage());

            _ = await response;
        }

        Task<ConsumeContext<PingMessage>> _ping;

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }
}
