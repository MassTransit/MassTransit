namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class When_a_message_is_published :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_received_by_the_queue()
        {
            ConsumeContext<A> context = await _received;

            context.Message.StringA.ShouldBe("ValueA");
        }

        [Test]
        public async Task Should_receive_the_inherited_version()
        {
            ConsumeContext<B> context = await _receivedB;

            context.Message.StringB.ShouldBe("ValueB");
        }

        Task<ConsumeContext<A>> _received;
        Task<ConsumeContext<B>> _receivedB;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _received = Handled<A>(configurator);
            _receivedB = Handled<B>(configurator);
        }

        [OneTimeSetUp]
        public async Task A_message_is_published()
        {
            await Bus.Publish(new A
            {
                StringA = "ValueA",
                StringB = "ValueB"
            });
        }


        class A :
            B
        {
            public string StringA { get; set; }
        }


        class B
        {
            public string StringB { get; set; }
        }
    }
}
