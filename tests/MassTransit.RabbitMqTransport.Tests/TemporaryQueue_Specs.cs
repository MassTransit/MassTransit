namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_temporary_queue_is_specified :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_able_to_request_response()
        {
            await Bus.Request<Request, Response>(InputQueueAddress, new Request(), TestCancellationToken, TestTimeout);
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handler<Request>(configurator, x => x.RespondAsync(new Response()));
        }


        class Request
        {
        }


        class Response
        {
        }
    }
}
