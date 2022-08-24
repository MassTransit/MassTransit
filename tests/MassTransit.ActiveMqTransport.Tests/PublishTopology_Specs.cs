namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using NUnit.Framework;
    using PublishContracts;


    namespace PublishContracts
    {
        public interface OrderSubmitted :
            OrderEvent
        {
        }


        public interface OrderEvent
        {
            public Guid OrderId { get; }
        }


        public interface PackageShipped :
            PackageEvent
        {
        }


        [ExcludeFromTopology]
        public interface PackageEvent
        {
        }


        public interface CustomerEvent
        {
            public Guid CustomerId { get; }
        }
    }


    public class Configuring_the_publish_topology_at_startup :
        ActiveMqTestFixture
    {
        [Test]
        [Explicit]
        public void Should_create_the_exchanges()
        {
        }

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.DeployPublishTopology = true;

            configurator.Publish<OrderSubmitted>();
            configurator.Publish<PackageShipped>();
        }
    }


    public class Configuring_the_publish_topology_at_startup_by_namespace :
        ActiveMqTestFixture
    {
        [Test]
        [Explicit]
        public void Should_create_the_exchanges()
        {
        }

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.DeployPublishTopology = true;

            configurator.AddPublishMessageTypesFromNamespaceContaining<OrderSubmitted>();
        }
    }
}
