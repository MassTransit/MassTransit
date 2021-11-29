namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Topology;
    using TopologyTestTypes;


    namespace TopologyTestTypes
    {
        public interface SingleInterface
        {
        }


        public interface FirstInterface
        {
        }


        public interface SecondInterface :
            FirstInterface
        {
        }


        public interface ThirdInterface :
            SecondInterface
        {
        }


        public interface AnotherThirdInterface :
            SecondInterface
        {
        }
    }


    [TestFixture]
    public class Publish_with_complex_hierarchy :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_received()
        {
            await Bus.Publish<ThirdInterface>(new { });
            await Bus.Publish<AnotherThirdInterface>(new { });

            ConsumeContext<FirstInterface> received = await _receivedA;
        }

        Task<ConsumeContext<FirstInterface>> _receivedA;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.PublishTopology.BrokerTopologyOptions = PublishBrokerTopologyOptions.MaintainHierarchy;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _receivedA = Handled<FirstInterface>(configurator);
        }
    }


    [TestFixture]
    public class Configuring_a_topology :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_not_consume_the_messages()
        {
            await Bus.Publish<ThirdInterface>(new { });

            await Task.Delay(1000);
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.DeployTopologyOnly = true;

            configurator.PublishTopology.BrokerTopologyOptions = PublishBrokerTopologyOptions.MaintainHierarchy;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            Handled<FirstInterface>(configurator);
        }
    }


    [TestFixture]
    public class Using_topology_to_bind_consumers
    {
        [Test]
        public void Should_include_a_binding_for_the_second_interface_only()
        {
            _consumeTopology.GetMessageTopology<SecondInterface>()
                .Bind();

            _consumeTopology.Apply(_builder);

            var topology = _builder.BuildBrokerTopology();

            var interfaceName = _nameFormatter.GetMessageName(typeof(SecondInterface)).ToString();

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == interfaceName), Is.True);
            Assert.That(topology.ExchangeBindings.Any(x => x.Source.ExchangeName == interfaceName && x.Destination.ExchangeName == _inputQueueName), Is.True);

            interfaceName = _nameFormatter.GetMessageName(typeof(FirstInterface)).ToString();

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == interfaceName), Is.False);
            Assert.That(topology.ExchangeBindings.Any(x => x.Source.ExchangeName == interfaceName && x.Destination.ExchangeName == _inputQueueName), Is.False);
        }

        [Test]
        public void Should_include_a_binding_for_the_single_interface()
        {
            _consumeTopology.GetMessageTopology<SingleInterface>()
                .Bind();

            _consumeTopology.Apply(_builder);

            var topology = _builder.BuildBrokerTopology();

            var singleInterfaceName = _nameFormatter.GetMessageName(typeof(SingleInterface)).ToString();

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == singleInterfaceName), Is.True);
            Assert.That(topology.ExchangeBindings.Any(x => x.Source.ExchangeName == singleInterfaceName && x.Destination.ExchangeName == _inputQueueName),
                Is.True);
        }

        [SetUp]
        public void Setup()
        {
            _nameFormatter = new RabbitMqMessageNameFormatter();
            _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(_nameFormatter);
            _consumeTopology = new RabbitMqConsumeTopology(RabbitMqBusFactory.MessageTopology, new RabbitMqPublishTopology(RabbitMqBusFactory.MessageTopology));

            _builder = new ReceiveEndpointBrokerTopologyBuilder();

            _inputQueueName = "input-queue";
            _builder.Queue = _builder.QueueDeclare(_inputQueueName, true, false, false, new Dictionary<string, object>());
            _builder.Exchange = _builder.ExchangeDeclare(_inputQueueName, _consumeTopology.ExchangeTypeSelector.DefaultExchangeType, true, false,
                new Dictionary<string, object>());

            _builder.QueueBind(_builder.Exchange, _builder.Queue, "", new Dictionary<string, object>());
        }

        RabbitMqMessageNameFormatter _nameFormatter;
        MessageNameFormatterEntityNameFormatter _entityNameFormatter;
        IRabbitMqConsumeTopologyConfigurator _consumeTopology;
        ReceiveEndpointBrokerTopologyBuilder _builder;
        string _inputQueueName;
    }


    [TestFixture]
    public class Using_flattened_topology_to_bind_publishers
    {
        [Test]
        public void Should_include_a_binding_for_the_second_interface_only()
        {
            _publishTopology.GetMessageTopology<SecondInterface>()
                .Apply(_builder);

            var topology = _builder.BuildBrokerTopology();

            var singleInterfaceName = _nameFormatter.GetMessageName(typeof(FirstInterface)).ToString();
            var interfaceName = _nameFormatter.GetMessageName(typeof(SecondInterface)).ToString();

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == interfaceName), Is.True);
            Assert.That(topology.Exchanges.Length, Is.EqualTo(2));
            Assert.That(topology.ExchangeBindings.Length, Is.EqualTo(1));
            Assert.That(topology.ExchangeBindings.Any(x => x.Source.ExchangeName == interfaceName && x.Destination.ExchangeName == singleInterfaceName),
                Is.True);

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == singleInterfaceName), Is.True);
        }

        [Test]
        public void Should_include_a_binding_for_the_single_interface()
        {
            _publishTopology.GetMessageTopology<SingleInterface>()
                .Apply(_builder);

            var topology = _builder.BuildBrokerTopology();

            var singleInterfaceName = _nameFormatter.GetMessageName(typeof(SingleInterface)).ToString();

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == singleInterfaceName), Is.True);
            Assert.That(topology.Exchanges.Length, Is.EqualTo(1));
            Assert.That(topology.ExchangeBindings.Length, Is.EqualTo(0));
        }

        [SetUp]
        public void Setup()
        {
            _nameFormatter = new RabbitMqMessageNameFormatter();
            _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(_nameFormatter);
            _publishTopology = new RabbitMqPublishTopology(RabbitMqBusFactory.MessageTopology);

            _builder = new PublishEndpointBrokerTopologyBuilder();
        }

        RabbitMqMessageNameFormatter _nameFormatter;
        MessageNameFormatterEntityNameFormatter _entityNameFormatter;
        IRabbitMqPublishTopologyConfigurator _publishTopology;
        PublishEndpointBrokerTopologyBuilder _builder;
    }


    [TestFixture]
    public class Using_hierarchical_topology_to_bind_publishers
    {
        [Test]
        public void Should_include_a_binding_for_the_second_interface_only()
        {
            _publishTopology.GetMessageTopology<SecondInterface>()
                .Apply(_builder);

            var topology = _builder.BuildBrokerTopology();
            topology.LogResult();

            var singleInterfaceName = _nameFormatter.GetMessageName(typeof(FirstInterface)).ToString();
            var interfaceName = _nameFormatter.GetMessageName(typeof(SecondInterface)).ToString();

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == interfaceName), Is.True);
            Assert.That(topology.Exchanges.Length, Is.EqualTo(2));
            Assert.That(topology.ExchangeBindings.Length, Is.EqualTo(1));
            Assert.That(topology.ExchangeBindings.Any(x => x.Source.ExchangeName == interfaceName && x.Destination.ExchangeName == singleInterfaceName),
                Is.True);

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == singleInterfaceName), Is.True);
        }

        [Test]
        public void Should_include_a_binding_for_the_single_interface()
        {
            _publishTopology.GetMessageTopology<SingleInterface>()
                .Apply(_builder);

            var topology = _builder.BuildBrokerTopology();
            topology.LogResult();

            var singleInterfaceName = _nameFormatter.GetMessageName(typeof(SingleInterface)).ToString();

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == singleInterfaceName), Is.True);
            Assert.That(topology.Exchanges.Length, Is.EqualTo(1));
            Assert.That(topology.ExchangeBindings.Length, Is.EqualTo(0));
        }

        [Test]
        public void Should_include_a_binding_for_the_third_interface_as_well()
        {
            _publishTopology.GetMessageTopology<ThirdInterface>()
                .Apply(_builder);

            var topology = _builder.BuildBrokerTopology();
            topology.LogResult();

            var firstInterfaceName = _nameFormatter.GetMessageName(typeof(FirstInterface)).ToString();
            var secondInterfaceName = _nameFormatter.GetMessageName(typeof(SecondInterface)).ToString();
            var thirdInterfaceName = _nameFormatter.GetMessageName(typeof(ThirdInterface)).ToString();

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == secondInterfaceName), Is.True);
            Assert.That(topology.Exchanges.Length, Is.EqualTo(3));
            Assert.That(topology.ExchangeBindings.Length, Is.EqualTo(2));
            Assert.That(topology.ExchangeBindings.Any(x => x.Source.ExchangeName == secondInterfaceName && x.Destination.ExchangeName == firstInterfaceName),
                Is.True);

            Assert.That(topology.ExchangeBindings.Any(x => x.Source.ExchangeName == thirdInterfaceName && x.Destination.ExchangeName == secondInterfaceName),
                Is.True);

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == firstInterfaceName), Is.True);
        }

        [SetUp]
        public void Setup()
        {
            _nameFormatter = new RabbitMqMessageNameFormatter();
            _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(_nameFormatter);
            _publishTopology = new RabbitMqPublishTopology(RabbitMqBusFactory.MessageTopology);

            _builder = new PublishEndpointBrokerTopologyBuilder(PublishBrokerTopologyOptions.MaintainHierarchy);
        }

        RabbitMqMessageNameFormatter _nameFormatter;
        MessageNameFormatterEntityNameFormatter _entityNameFormatter;
        IRabbitMqPublishTopologyConfigurator _publishTopology;
        PublishEndpointBrokerTopologyBuilder _builder;
    }
}
