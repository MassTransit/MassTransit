namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using ConsumerBind_Specs;
    using NUnit.Framework;


    [TestFixture]
    public class Sending_to_a_publish_exchange :
        ConsumerBindingTestFixture
    {
        [Test]
        public async Task Should_arrive_on_the_receive_endpoint()
        {
            var destinationAddress = Bus.GetRabbitMqHostTopology().GetDestinationAddress(typeof(A));

            var endpoint = await Bus.GetSendEndpoint(destinationAddress);

            await endpoint.Send(new A());

            await _handled;
        }

        Task<ConsumeContext<A>> _handled;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);
        }
    }
}
