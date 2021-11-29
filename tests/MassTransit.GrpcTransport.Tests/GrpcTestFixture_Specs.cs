namespace MassTransit.GrpcTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Sending_a_message_to_the_endpoint :
        GrpcTestFixture
    {
        [Test]
        public async Task Should_be_received_by_the_handler()
        {
            await InputQueueSendEndpoint.Send(new A());

            await _receivedA;
        }

        [Test]
        public void Should_start_the_handler_properly()
        {
        }

        Task<ConsumeContext<A>> _receivedA;


        class A
        {
        }


        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            _receivedA = Handler<A>(configurator, async context => Console.WriteLine("Hi"));
        }
    }


    [TestFixture]
    public class Starting_up_the_client_test_fixture :
        GrpcClientTestFixture
    {
        [Test]
        public void Should_be_successful()
        {
        }

        [Test]
        public async Task Should_cross_the_border()
        {
            await ClientBus.Publish(new A());

            await _receivedA;
        }

        Task<ConsumeContext<A>> _receivedA;


        class A
        {
        }


        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            _receivedA = Handler<A>(configurator, async context => Console.WriteLine("Hi"));
        }
    }
}
