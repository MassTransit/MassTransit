namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class When_a_message_consumer_throws_an_exception :
        RabbitMqTestFixture
    {
        A _message;

        TaskCompletionSource<A> _received;

        [Test]
        public async Task Should_be_received_by_the_handler()
        {
            Task<ConsumeContext<Fault<A>>> faultHandled = SubscribeHandler<Fault<A>>();

            _message = new A {StringA = "ValueA"};

            await InputQueueSendEndpoint.Send(_message, Pipe.Execute<SendContext>(x => x.FaultAddress = BusAddress));

            await _received.Task;

            ConsumeContext<Fault<A>> fault = await faultHandled;

            fault.Message.Message.StringA.ShouldBe("ValueA");
        }

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


        public class A :
            CorrelatedBy<Guid>
        {
            public string StringA { get; set; }

            public Guid CorrelationId => Guid.Empty;
        }
    }
}
