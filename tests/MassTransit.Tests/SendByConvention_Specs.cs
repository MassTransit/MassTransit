namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class SendByConvention_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_send_by_convention_to_the_input_queue()
        {
            await Bus.Send(new NastyMessage {Value = "Hello"});

            await _handled;
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<NastyMessage>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<NastyMessage>(configurator);

            EndpointConvention.Map<NastyMessage>(configurator.InputAddress);
        }


        class NastyMessage
        {
            public string Value { get; set; }
        }
    }


    [TestFixture]
    public class Sending_using_a_short_address :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_send_by_convention_to_the_input_queue()
        {
            await Bus.Send(new TastyMessage {Value = "Hello"});

            await _handled;
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<TastyMessage>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<TastyMessage>(configurator);

            EndpointConvention.Map<TastyMessage>(new Uri($"queue:{InputQueueName}"));
        }


        class TastyMessage
        {
            public string Value { get; set; }
        }
    }


    [TestFixture]
    public class Conventional_polymorphic :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_send_by_convention_to_the_input_queue()
        {
            await Bus.Send(new NastyEvent
            {
                Timestamp = DateTime.UtcNow,
                Value = "Hello"
            });

            await _handled;
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<NastyEvent>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<NastyEvent>(configurator);

            EndpointConvention.Map<BusinessEvent>(configurator.InputAddress);
        }


        class NastyEvent :
            BusinessEvent
        {
            public string Value { get; set; }
            public DateTime Timestamp { get; set; }
        }


        interface BusinessEvent
        {
            DateTime Timestamp { get; }
        }
    }


    [TestFixture]
    public class Conventional_polymorphic_base_class :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_send_by_convention_to_the_input_queue()
        {
            await Bus.Send(new NastyEvent
            {
                Timestamp = DateTime.UtcNow,
                Value = "Hello"
            });

            await _handled;
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<NastyEvent>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<NastyEvent>(configurator);

            EndpointConvention.Map<BusinessEvent>(configurator.InputAddress);
        }


        class NastyEvent :
            BusinessEvent
        {
            public string Value { get; set; }
            public DateTime Timestamp { get; set; }
        }


        class BusinessEvent
        {
            DateTime Timestamp { get; }
        }
    }


    [TestFixture]
    public class Conventional_polymorphic_overridden :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_send_by_convention_to_the_input_queue()
        {
            await Bus.Send(new NastyEvent
            {
                Timestamp = DateTime.UtcNow,
                Value = "Hello"
            });

            await _handled;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("second_queue", x =>
            {
                _handled = Handled<NastyEvent>(x);

                EndpointConvention.Map<NastyEvent>(x.InputAddress);
            });
        }

        Task<ConsumeContext<NastyEvent>> _handled;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            EndpointConvention.Map<BusinessEvent>(configurator.InputAddress);
        }


        public class NastyEvent :
            BusinessEvent
        {
            public string Value { get; set; }
            public DateTime Timestamp { get; set; }
        }


        public interface BusinessEvent
        {
            DateTime Timestamp { get; }
        }
    }
}
