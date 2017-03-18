// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Topology;
    using NUnit.Framework;
    using Topology;
    using Topology.Builders;
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

            var topology = _builder.BuildTopologyLayout();

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

            var topology = _builder.BuildTopologyLayout();

            var singleInterfaceName = _nameFormatter.GetMessageName(typeof(SingleInterface)).ToString();

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == singleInterfaceName), Is.True);
            Assert.That(topology.ExchangeBindings.Any(x => x.Source.ExchangeName == singleInterfaceName && x.Destination.ExchangeName == _inputQueueName), Is.True);
        }

        [SetUp]
        public void Setup()
        {
            _nameFormatter = new RabbitMqMessageNameFormatter();
            _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(_nameFormatter);
            _consumeTopology = new RabbitMqConsumeTopology(_entityNameFormatter);

            _builder = new ReceiveEndpointConsumeTopologyBuilder();

            _inputQueueName = "input-queue";
            _builder.Queue = _builder.QueueDeclare(_inputQueueName, true, false, false, new Dictionary<string, object>());
            _builder.Exchange = _builder.ExchangeDeclare(_inputQueueName, _consumeTopology.ExchangeTypeSelector.DefaultExchangeType, true, false,
                new Dictionary<string, object>());
            _builder.QueueBind(_builder.Exchange, _builder.Queue, "", new Dictionary<string, object>());
        }

        RabbitMqMessageNameFormatter _nameFormatter;
        MessageNameFormatterEntityNameFormatter _entityNameFormatter;
        IRabbitMqConsumeTopologyConfigurator _consumeTopology;
        ReceiveEndpointConsumeTopologyBuilder _builder;
        string _inputQueueName;
    }


    [TestFixture]
    public class Using_flattened_topology_to_bind_publishers
    {
        [Test]
        public void Should_include_a_binding_for_the_second_interface_only()
        {
            _publishTopology.GetMessageTopology<SecondInterface>()
                .ApplyMessageTopology(_builder);

            var topology = _builder.BuildTopologyLayout();

            var singleInterfaceName = _nameFormatter.GetMessageName(typeof(FirstInterface)).ToString();
            var interfaceName = _nameFormatter.GetMessageName(typeof(SecondInterface)).ToString();

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == interfaceName), Is.True);
            Assert.That(topology.Exchanges.Length, Is.EqualTo(2));
            Assert.That(topology.ExchangeBindings.Length, Is.EqualTo(1));
            Assert.That(topology.ExchangeBindings.Any(x => x.Source.ExchangeName == interfaceName && x.Destination.ExchangeName == singleInterfaceName), Is.True);

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == singleInterfaceName), Is.True);
        }

        [Test]
        public void Should_include_a_binding_for_the_single_interface()
        {
            _publishTopology.GetMessageTopology<SingleInterface>()
                .ApplyMessageTopology(_builder);

            var topology = _builder.BuildTopologyLayout();

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
            _publishTopology = new RabbitMqPublishTopology(_entityNameFormatter);

            _builder = new PublishEndpointTopologyBuilder();
        }

        RabbitMqMessageNameFormatter _nameFormatter;
        MessageNameFormatterEntityNameFormatter _entityNameFormatter;
        IRabbitMqPublishTopologyConfigurator _publishTopology;
        PublishEndpointTopologyBuilder _builder;
    }


    [TestFixture]
    public class Using_hierarchical_topology_to_bind_publishers
    {
        [Test]
        public void Should_include_a_binding_for_the_second_interface_only()
        {
            _publishTopology.GetMessageTopology<SecondInterface>()
                .ApplyMessageTopology(_builder);

            var topology = _builder.BuildTopologyLayout();
            topology.LogResult();

            var singleInterfaceName = _nameFormatter.GetMessageName(typeof(FirstInterface)).ToString();
            var interfaceName = _nameFormatter.GetMessageName(typeof(SecondInterface)).ToString();

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == interfaceName), Is.True);
            Assert.That(topology.Exchanges.Length, Is.EqualTo(2));
            Assert.That(topology.ExchangeBindings.Length, Is.EqualTo(1));
            Assert.That(topology.ExchangeBindings.Any(x => x.Source.ExchangeName == interfaceName && x.Destination.ExchangeName == singleInterfaceName), Is.True);

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == singleInterfaceName), Is.True);
        }

        [Test]
        public void Should_include_a_binding_for_the_third_interface_as_well()
        {
            _publishTopology.GetMessageTopology<ThirdInterface>()
                .ApplyMessageTopology(_builder);

            var topology = _builder.BuildTopologyLayout();
            topology.LogResult();

            var firstInterfaceName = _nameFormatter.GetMessageName(typeof(FirstInterface)).ToString();
            var secondInterfaceName = _nameFormatter.GetMessageName(typeof(SecondInterface)).ToString();
            var thirdInterfaceName = _nameFormatter.GetMessageName(typeof(ThirdInterface)).ToString();

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == secondInterfaceName), Is.True);
            Assert.That(topology.Exchanges.Length, Is.EqualTo(3));
            Assert.That(topology.ExchangeBindings.Length, Is.EqualTo(2));
            Assert.That(topology.ExchangeBindings.Any(x => x.Source.ExchangeName == secondInterfaceName && x.Destination.ExchangeName == firstInterfaceName), Is.True);
            Assert.That(topology.ExchangeBindings.Any(x => x.Source.ExchangeName == thirdInterfaceName && x.Destination.ExchangeName == secondInterfaceName), Is.True);

            Assert.That(topology.Exchanges.Any(x => x.ExchangeName == firstInterfaceName), Is.True);
        }

        [Test]
        public void Should_include_a_binding_for_the_single_interface()
        {
            _publishTopology.GetMessageTopology<SingleInterface>()
                .ApplyMessageTopology(_builder);

            var topology = _builder.BuildTopologyLayout();
            topology.LogResult();

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
            _publishTopology = new RabbitMqPublishTopology(_entityNameFormatter);

            _builder = new PublishEndpointTopologyBuilder(PublishEndpointTopologyBuilder.Options.MaintainHierarchy);
        }

        RabbitMqMessageNameFormatter _nameFormatter;
        MessageNameFormatterEntityNameFormatter _entityNameFormatter;
        IRabbitMqPublishTopologyConfigurator _publishTopology;
        PublishEndpointTopologyBuilder _builder;
    }
}