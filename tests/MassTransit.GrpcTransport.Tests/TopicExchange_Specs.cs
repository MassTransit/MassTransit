namespace MassTransit.GrpcTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using Transports.Fabric;


    public class Using_a_hash_topic_pattern :
        GrpcTestFixture
    {
        Task<ConsumeContext<A>> _handled;

        public Using_a_hash_topic_pattern()
        {
            TestTimeout = TimeSpan.FromSeconds(5);
        }

        [Test]
        [Explicit]
        public void Should_wonderful_display()
        {
            var result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        [Test]
        public async Task Should_match_the_endpoint_binding()
        {
            var endpoint = await Bus.GetSendEndpoint(new Uri("exchange:test-exchange?type=topic"));

            await endpoint.Send(new A(), x => x.SetRoutingKey("alpha"));

            await _handled;
        }

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _handled = Handled<A>(configurator);

            configurator.Bind("test-exchange", ExchangeType.Topic, "#");
        }


        public class A
        {
            public string Value { get; set; }
        }
    }


    public class Using_a_wildcard_topic_pattern :
        GrpcTestFixture
    {
        Task<ConsumeContext<A>> _handled;

        public Using_a_wildcard_topic_pattern()
        {
            TestTimeout = TimeSpan.FromSeconds(5);
        }

        [Test]
        public async Task Should_match_the_endpoint_binding()
        {
            var endpoint = await Bus.GetSendEndpoint(new Uri("exchange:test-exchange?type=topic"));

            await endpoint.Send(new A { Value = "Bad" }, x => x.SetRoutingKey("bus.red"));
            await endpoint.Send(new A { Value = "Bad" }, x => x.SetRoutingKey("bus.green"));
            await endpoint.Send(new A { Value = "Good" }, x => x.SetRoutingKey("car.blue"));

            ConsumeContext<A> handled = await _handled;

            Assert.That(handled.Message.Value, Is.EqualTo("Good"));
        }

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _handled = Handled<A>(configurator);

            configurator.Bind("test-exchange", ExchangeType.Topic, "car.*");
        }


        public class A
        {
            public string Value { get; set; }
        }
    }


    public class Using_a_wildcard_topic_pattern_too :
        GrpcTestFixture
    {
        Task<ConsumeContext<A>> _handled;

        public Using_a_wildcard_topic_pattern_too()
        {
            TestTimeout = TimeSpan.FromSeconds(5);
        }

        [Test]
        public async Task Should_match_the_endpoint_binding()
        {
            var endpoint = await Bus.GetSendEndpoint(new Uri("exchange:test-exchange?type=topic"));

            await endpoint.Send(new A { Value = "Bad" }, x => x.SetRoutingKey("bus.red.large"));
            await endpoint.Send(new A { Value = "Bad" }, x => x.SetRoutingKey("car.green.small"));
            await endpoint.Send(new A { Value = "Bad" }, x => x.SetRoutingKey("bus.green.small"));
            await endpoint.Send(new A { Value = "Good" }, x => x.SetRoutingKey("car.blue.large"));

            ConsumeContext<A> handled = await _handled;

            Assert.That(handled.Message.Value, Is.EqualTo("Good"));
        }

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _handled = Handled<A>(configurator);

            configurator.Bind("test-exchange", ExchangeType.Topic, "car.*.large");
        }


        public class A
        {
            public string Value { get; set; }
        }
    }


    public class Using_a_wildcard_topic_pattern_too_via_client :
        GrpcClientTestFixture
    {
        Task<ConsumeContext<A>> _handled;

        public Using_a_wildcard_topic_pattern_too_via_client()
        {
            TestTimeout = TimeSpan.FromSeconds(5);
        }

        [Test]
        public async Task Should_match_the_endpoint_binding()
        {
            var endpoint = await ClientBus.GetSendEndpoint(new Uri("exchange:test-exchange?type=topic"));

            await endpoint.Send(new A { Value = "Bad" }, x => x.SetRoutingKey("bus.red.large"));
            await endpoint.Send(new A { Value = "Bad" }, x => x.SetRoutingKey("car.green.small"));
            await endpoint.Send(new A { Value = "Bad" }, x => x.SetRoutingKey("bus.green.small"));
            await endpoint.Send(new A { Value = "Good" }, x => x.SetRoutingKey("car.blue.large"));

            ConsumeContext<A> handled = await _handled;

            Assert.That(handled.Message.Value, Is.EqualTo("Good"));
        }

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _handled = Handled<A>(configurator);

            configurator.Bind("test-exchange", ExchangeType.Topic, "car.*.large");
        }


        public class A
        {
            public string Value { get; set; }
        }
    }
}
