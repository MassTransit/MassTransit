namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Sending_a_message_to_the_endpoint :
        InMemoryTestFixture
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

        #pragma warning disable NUnit1032
        Task<ConsumeContext<A>> _receivedA;
        #pragma warning restore NUnit1032


        class A
        {
        }


        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _receivedA = Handler<A>(configurator, async context => Console.WriteLine("Hi"));
        }
    }


    [TestFixture]
    public class Sending_a_skipped_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_hang_the_shutdown()
        {
            await InputQueueSendEndpoint.Send(new B());

            await InputQueueSendEndpoint.Send(new A());

            await _receivedA;
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<A>> _receivedA;
        #pragma warning restore NUnit1032


        class A
        {
        }


        class B
        {
        }


        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _receivedA = Handler<A>(configurator, context => Console.Out.WriteLineAsync("Hi"));
        }
    }


    [TestFixture]
    public class Sending_an_object_to_the_endpoint :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_by_the_handler()
        {
            object message = new A();

            await InputQueueSendEndpoint.Send(message);

            await _receivedA;
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<A>> _receivedA;
        #pragma warning restore NUnit1032


        class A
        {
        }


        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _receivedA = Handled<A>(configurator);
        }
    }
}
