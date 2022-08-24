namespace MassTransit.RabbitMqTransport.Tests
{
    using NUnit.Framework;
    using PublishContracts;


    namespace PublishContracts
    {
        using System;


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
        RabbitMqTestFixture
    {
        [Test]
        [Explicit]
        public void Should_create_the_exchanges()
        {
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.DeployPublishTopology = true;

            configurator.Publish<OrderSubmitted>();
            configurator.Publish<PackageShipped>();
        }
    }


    public class Configuring_the_publish_topology_at_startup_by_namespace :
        RabbitMqTestFixture
    {
        [Test]
        [Explicit]
        public void Should_create_the_exchanges()
        {
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.DeployPublishTopology = true;

            configurator.AddPublishMessageTypesFromNamespaceContaining<OrderSubmitted>();
        }
    }
}
