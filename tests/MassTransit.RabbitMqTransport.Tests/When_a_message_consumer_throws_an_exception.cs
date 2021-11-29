namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    [Category("Flaky")]
    public class When_a_message_consumer_throws_an_exception :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_received_by_the_handler()
        {
            Task<ConsumeContext<Fault<A>>> faultHandled = SubscribeHandler<Fault<A>>();

            _message = new A { StringA = "ValueA" };

            await InputQueueSendEndpoint.Send(_message, Pipe.Execute<SendContext>(x => x.FaultAddress = BusAddress));

            await _received.Task;

            ConsumeContext<Fault<A>> fault = await faultHandled;

            fault.Message.Message.StringA.ShouldBe("ValueA");
        }

        A _message;

        TaskCompletionSource<A> _received;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.AutoStart = true;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _received = GetTask<A>();

            Handler<A>(configurator, async context =>
            {
                _received.TrySetResult(context.Message);

                throw new IntentionalTestException("This is supposed to happen");
            });
        }


        public class A
        {
            public string StringA { get; set; }
        }
    }
}
