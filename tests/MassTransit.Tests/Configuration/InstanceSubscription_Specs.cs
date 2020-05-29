namespace MassTransit.Tests.Configuration
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Pipeline;
    using TestFramework;
    using TestFramework.Messages;


    public class When_subscribing_an_object_instance_to_the_bus :
        InMemoryTestFixture
    {
        OneMessageConsumer _consumer;
        MessageA _message;

        [Test]
        public async Task Should_have_received_the_message()
        {
            _message = new MessageA();

            await Bus.Publish(_message);

            await _consumer.Task;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new OneMessageConsumer(GetTask<MessageA>());

            object instance = _consumer;

            configurator.Instance(instance);
        }
    }


    public class When_subscribing_a_consumer_to_the_bus_by_factory_method :
        InMemoryTestFixture
    {
        OneMessageConsumer _consumer;
        MessageA _message;

        [Test]
        public async Task Should_have_received_the_message()
        {
            _message = new MessageA();

            await Bus.Publish(_message);

            await _consumer.Task;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new OneMessageConsumer(GetTask<MessageA>());


            configurator.Consumer(() => _consumer);
        }
    }


    public class When_subscribing_a_consumer_to_the_bus_by_object_factory_method :
        InMemoryTestFixture
    {
        OneMessageConsumer _consumer;
        MessageA _message;

        [Test]
        public async Task Should_have_received_the_message()
        {
            _message = new MessageA();

            await Bus.Publish(_message);

            await _consumer.Task;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _consumer = new OneMessageConsumer(GetTask<MessageA>());

            configurator.Consumer(typeof(OneMessageConsumer), type => _consumer);
        }
    }
}
