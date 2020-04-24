namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Topology.EntityNameFormatters;
    using NUnit.Framework;
    using Topology;
    using Topology.Builders;
    using Topology.Configurators;
    using Topology.Topologies;
    using TopologyTestTypes;


    namespace TopologyTestTypes
    {
        public interface SingleInterface
        {
        }


        public interface FirstInterface
        {
            string Value { get; }
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
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_be_received()
        {
            await Bus.Publish<ThirdInterface>(new {Value = "A"});
            await Bus.Publish<AnotherThirdInterface>(new {Value = "B"});

            var received = await _receivedA;

            await Task.Delay(10000);

            Assert.That(_count, Is.EqualTo(2));
        }

        Task<ConsumeContext<FirstInterface>> _receivedA;
        Task<ConsumeContext<FirstInterface>> _receivedB;
        int _count;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            base.ConfigureServiceBusReceiveEndpoint(configurator);

            configurator.EnableDuplicateDetection(TimeSpan.FromSeconds(30));

            _receivedA = Handled<FirstInterface>(configurator, context => context.Message.Value == "A");
            _receivedB = Handled<FirstInterface>(configurator, context => context.Message.Value == "B");

            configurator.Handler<FirstInterface>(async context => Interlocked.Increment(ref _count));
        }
    }


    [TestFixture]
    public class Configuring_a_topology :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_not_consume_the_messages()
        {
            await Bus.Publish<ThirdInterface>(new { });

            await Task.Delay(1000);
        }

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.DeployTopologyOnly = true;
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            base.ConfigureServiceBusReceiveEndpoint(configurator);

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
                .Subscribe("ToTheSecondInterface");

            _consumeTopology.Apply(_builder);

            var topology = _builder.BuildBrokerTopology();

            var interfaceName = _nameFormatter.GetMessageName(typeof(SecondInterface)).ToString();

            Assert.That(topology.Topics.Any(x => x.TopicDescription.Path == interfaceName), Is.True);
            Assert.That(
                topology.QueueSubscriptions.Any(x => x.Source.TopicDescription.Path == interfaceName && x.Destination.QueueDescription.Path == _inputQueueName),
                Is.True);

            interfaceName = _nameFormatter.GetMessageName(typeof(FirstInterface)).ToString();

            Assert.That(topology.Topics.Any(x => x.TopicDescription.Path == interfaceName), Is.False);
            Assert.That(
                topology.QueueSubscriptions.Any(x => x.Source.TopicDescription.Path == interfaceName && x.Destination.QueueDescription.Path == _inputQueueName),
                Is.False);
        }

        [Test]
        public void Should_include_a_binding_for_the_single_interface()
        {
            _consumeTopology.GetMessageTopology<SingleInterface>()
                .Subscribe("ToTheSingleInterface");

            _consumeTopology.Apply(_builder);

            var topology = _builder.BuildBrokerTopology();

            var singleInterfaceName = _nameFormatter.GetMessageName(typeof(SingleInterface)).ToString();

            Assert.That(topology.Topics.Any(x => x.TopicDescription.Path == singleInterfaceName), Is.True);
            Assert.That(
                topology.QueueSubscriptions.Any(x =>
                    x.Source.TopicDescription.Path == singleInterfaceName && x.Destination.QueueDescription.Path == _inputQueueName),
                Is.True);
        }

        [SetUp]
        public void Setup()
        {
            _nameFormatter = new ServiceBusMessageNameFormatter(false);
            _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(_nameFormatter);
            _consumeTopology = new ServiceBusConsumeTopology(AzureBusFactory.MessageTopology, new ServiceBusPublishTopology(AzureBusFactory.MessageTopology));

            _builder = new ReceiveEndpointBrokerTopologyBuilder();

            _inputQueueName = "input-queue";
            _builder.Queue = _builder.CreateQueue(new QueueConfigurator(_inputQueueName).GetQueueDescription());
        }

        ServiceBusMessageNameFormatter _nameFormatter;
        MessageNameFormatterEntityNameFormatter _entityNameFormatter;
        IServiceBusConsumeTopologyConfigurator _consumeTopology;
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

            Assert.That(topology.Topics.Any(x => x.TopicDescription.Path == interfaceName), Is.True);
            Assert.That(topology.Topics.Length, Is.EqualTo(2));
            Assert.That(topology.TopicSubscriptions.Length, Is.EqualTo(1));
            Assert.That(
                topology.TopicSubscriptions.Any(x =>
                    x.Source.TopicDescription.Path == interfaceName && x.Destination.TopicDescription.Path == singleInterfaceName), Is.True);

            Assert.That(topology.Topics.Any(x => x.TopicDescription.Path == singleInterfaceName), Is.True);
        }

        [Test]
        public void Should_include_a_binding_for_the_single_interface()
        {
            _publishTopology.GetMessageTopology<SingleInterface>()
                .Apply(_builder);

            var topology = _builder.BuildBrokerTopology();

            var singleInterfaceName = _nameFormatter.GetMessageName(typeof(SingleInterface)).ToString();

            Assert.That(topology.Topics.Any(x => x.TopicDescription.Path == singleInterfaceName), Is.True);
            Assert.That(topology.Topics.Length, Is.EqualTo(1));
            Assert.That(topology.TopicSubscriptions.Length, Is.EqualTo(0));
        }

        [SetUp]
        public void Setup()
        {
            _nameFormatter = new ServiceBusMessageNameFormatter(false);
            _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(_nameFormatter);
            _publishTopology = new ServiceBusPublishTopology(AzureBusFactory.MessageTopology);

            _builder = new PublishEndpointBrokerTopologyBuilder();
        }

        ServiceBusMessageNameFormatter _nameFormatter;
        MessageNameFormatterEntityNameFormatter _entityNameFormatter;
        IServiceBusPublishTopologyConfigurator _publishTopology;
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

            Assert.That(topology.Topics.Any(x => x.TopicDescription.Path == interfaceName), Is.True);
            Assert.That(topology.Topics.Length, Is.EqualTo(2));
            Assert.That(topology.TopicSubscriptions.Length, Is.EqualTo(1));
            Assert.That(
                topology.TopicSubscriptions.Any(x =>
                    x.Source.TopicDescription.Path == interfaceName && x.Destination.TopicDescription.Path == singleInterfaceName), Is.True);

            Assert.That(topology.Topics.Any(x => x.TopicDescription.Path == singleInterfaceName), Is.True);
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

            Assert.That(topology.Topics.Any(x => x.TopicDescription.Path == secondInterfaceName), Is.True);
            Assert.That(topology.Topics.Length, Is.EqualTo(3));
            Assert.That(topology.TopicSubscriptions.Length, Is.EqualTo(2));
            Assert.That(
                topology.TopicSubscriptions.Any(x =>
                    x.Source.TopicDescription.Path == secondInterfaceName && x.Destination.TopicDescription.Path == firstInterfaceName),
                Is.True);

            Assert.That(
                topology.TopicSubscriptions.Any(x =>
                    x.Source.TopicDescription.Path == thirdInterfaceName && x.Destination.TopicDescription.Path == secondInterfaceName),
                Is.True);

            Assert.That(topology.Topics.Any(x => x.TopicDescription.Path == firstInterfaceName), Is.True);
        }

        [Test]
        public void Should_include_a_binding_for_the_single_interface()
        {
            _publishTopology.GetMessageTopology<SingleInterface>()
                .Apply(_builder);

            var topology = _builder.BuildBrokerTopology();
            topology.LogResult();

            var singleInterfaceName = _nameFormatter.GetMessageName(typeof(SingleInterface)).ToString();

            Assert.That(topology.Topics.Any(x => x.TopicDescription.Path == singleInterfaceName), Is.True);
            Assert.That(topology.Topics.Length, Is.EqualTo(1));
            Assert.That(topology.TopicSubscriptions.Length, Is.EqualTo(0));
        }

        [SetUp]
        public void Setup()
        {
            _nameFormatter = new ServiceBusMessageNameFormatter(false);
            _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(_nameFormatter);
            _publishTopology = new ServiceBusPublishTopology(AzureBusFactory.MessageTopology);

            _builder = new PublishEndpointBrokerTopologyBuilder();
        }

        ServiceBusMessageNameFormatter _nameFormatter;
        MessageNameFormatterEntityNameFormatter _entityNameFormatter;
        IServiceBusPublishTopologyConfigurator _publishTopology;
        PublishEndpointBrokerTopologyBuilder _builder;
    }
}
