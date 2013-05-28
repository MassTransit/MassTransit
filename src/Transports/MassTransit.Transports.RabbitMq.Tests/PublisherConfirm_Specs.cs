namespace MassTransit.Transports.RabbitMq.Tests
{
    using BusConfigurators;
    using Configuration.Configurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class PublisherConfirm_Specs:
        Given_two_rabbitmq_buses_walk_into_a_bar
    {
        [Test]
        public void Should_call_the_ack_method_upon_delivery()
        {
            RemoteBus.Publish(new A
                {
                    StringA = "ValueA",
                });

            _received.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
        }

        Future<A> _received;

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);


            _received = new Future<A>();

            configurator.Subscribe(s => s.Handler<A>(message => _received.Complete(message)));
        }

        protected override void ConfigureRabbitMq(RabbitMqTransportFactoryConfigurator configurator)
        {
            base.ConfigureRabbitMq(configurator);
        }

        class A
        {
            public string StringA { get; set; }
        }

        class B
        {
            public string StringB { get; set; }
        }
    }
}
