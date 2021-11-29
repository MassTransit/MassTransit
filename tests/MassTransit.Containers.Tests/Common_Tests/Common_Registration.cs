namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    public class Common_Registration<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_come_from_the_default_address()
        {
            Task<ConsumeContext<PlainEvent>> handled = await ConnectPublishHandler<PlainEvent>();

            await Bus.Publish(new PlainCommand());

            ConsumeContext<PlainEvent> context = await handled;

            var expected = new Uri(InMemoryTestHarness.BaseAddress, EndpointNameFormatter.Consumer<PlainConsumer>());
            Assert.That(context.SourceAddress, Is.EqualTo(expected));
        }

        [Test]
        public async Task Should_come_from_the_definition_address()
        {
            Task<ConsumeContext<ByDefinitionEvent>> handled = await ConnectPublishHandler<ByDefinitionEvent>();

            await Bus.Publish(new ByDefinitionCommand());

            ConsumeContext<ByDefinitionEvent> context = await handled;

            var expected = new Uri(InMemoryTestHarness.BaseAddress, "by_definition");
            Assert.That(context.SourceAddress, Is.EqualTo(expected));
        }

        [Test]
        public async Task Should_come_from_the_endpoint_address()
        {
            Task<ConsumeContext<ByEndpointEvent>> handled = await ConnectPublishHandler<ByEndpointEvent>();

            await Bus.Publish(new ByEndpointCommand());

            ConsumeContext<ByEndpointEvent> context = await handled;

            var expected = new Uri(InMemoryTestHarness.BaseAddress, "by-endpoint");
            Assert.That(context.SourceAddress, Is.EqualTo(expected));
        }

        [Test]
        public async Task Should_come_from_the_endpoint_name()
        {
            Task<ConsumeContext<ByEndpointNameEvent>> handled = await ConnectPublishHandler<ByEndpointNameEvent>();

            await Bus.Publish(new ByEndpointNameCommand());

            ConsumeContext<ByEndpointNameEvent> context = await handled;

            var expected = new Uri(InMemoryTestHarness.BaseAddress, "by_endpoint_name");
            Assert.That(context.SourceAddress, Is.EqualTo(expected));
        }

        [Test]
        public async Task Should_come_from_the_endpoint_override_address()
        {
            Task<ConsumeContext<ByEndpointDefinitionEvent>> handled = await ConnectPublishHandler<ByEndpointDefinitionEvent>();

            await Bus.Publish(new ByEndpointDefinitionCommand());

            ConsumeContext<ByEndpointDefinitionEvent> context = await handled;

            var expected = new Uri(InMemoryTestHarness.BaseAddress, "by_endpoint_definition");
            Assert.That(context.SourceAddress, Is.EqualTo(expected));
        }

        [Test]
        public async Task Should_come_from_the_override_name()
        {
            Task<ConsumeContext<ByEndpointOverrideEvent>> handled = await ConnectPublishHandler<ByEndpointOverrideEvent>();

            await Bus.Publish(new ByEndpointOverrideCommand());

            ConsumeContext<ByEndpointOverrideEvent> context = await handled;

            var expected = new Uri(InMemoryTestHarness.BaseAddress, "by_endpoint_override");
            Assert.That(context.SourceAddress, Is.EqualTo(expected));
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<PlainConsumer>();

            configurator.AddConsumer<ByDefinitionConsumer, ByDefinitionConsumerDefinition>();

            configurator.AddConsumer<ByEndpointConsumer>()
                .Endpoint(x => x.Name = "by-endpoint");

            configurator.AddConsumer<ByEndpointDefinitionConsumer, ByEndpointDefinitionConsumerDefinition>()
                .Endpoint(x => x.Name = "by_endpoint_definition");

            configurator.AddConsumer<ByEndpointNameConsumer, ByEndpointNameConsumerDefinition>()
                .Endpoint(x => x.Name = "by_endpoint_name");

            configurator.AddConsumer<ByEndpointOverrideConsumer, ByEndpointOverrideConsumerDefinition>()
                .Endpoint(x => x.Name = "by_endpoint_override");
        }


        class PlainConsumer :
            IConsumer<PlainCommand>
        {
            public async Task Consume(ConsumeContext<PlainCommand> context)
            {
                await context.Publish(new PlainEvent());
            }
        }


        class PlainCommand
        {
        }


        class PlainEvent
        {
        }


        class ByEndpointConsumer :
            IConsumer<ByEndpointCommand>
        {
            public async Task Consume(ConsumeContext<ByEndpointCommand> context)
            {
                await context.Publish(new ByEndpointEvent());
            }
        }


        class ByEndpointCommand
        {
        }


        class ByEndpointEvent
        {
        }


        class ByDefinitionConsumer :
            IConsumer<ByDefinitionCommand>
        {
            public async Task Consume(ConsumeContext<ByDefinitionCommand> context)
            {
                await context.Publish(new ByDefinitionEvent());
            }
        }


        class ByDefinitionConsumerDefinition :
            ConsumerDefinition<ByDefinitionConsumer>
        {
            public ByDefinitionConsumerDefinition()
            {
                EndpointName = "by_definition";
            }
        }


        class ByDefinitionCommand
        {
        }


        class ByDefinitionEvent
        {
        }


        class ByEndpointDefinitionConsumer :
            IConsumer<ByEndpointDefinitionCommand>
        {
            public async Task Consume(ConsumeContext<ByEndpointDefinitionCommand> context)
            {
                await context.Publish(new ByEndpointDefinitionEvent());
            }
        }


        class ByEndpointDefinitionConsumerDefinition :
            ConsumerDefinition<ByEndpointDefinitionConsumer>
        {
            public ByEndpointDefinitionConsumerDefinition()
            {
                Endpoint(x => x.Name = "by_endpoint_definition");
            }
        }


        class ByEndpointDefinitionCommand
        {
        }


        class ByEndpointDefinitionEvent
        {
        }


        class ByEndpointOverrideConsumer :
            IConsumer<ByEndpointOverrideCommand>
        {
            public async Task Consume(ConsumeContext<ByEndpointOverrideCommand> context)
            {
                await context.Publish(new ByEndpointOverrideEvent());
            }
        }


        class ByEndpointOverrideConsumerDefinition :
            ConsumerDefinition<ByEndpointOverrideConsumer>
        {
            public ByEndpointOverrideConsumerDefinition()
            {
                Endpoint(x => x.Name = "not_by_endpoint_definition");
            }
        }


        class ByEndpointOverrideCommand
        {
        }


        class ByEndpointOverrideEvent
        {
        }


        class ByEndpointNameConsumer :
            IConsumer<ByEndpointNameCommand>
        {
            public async Task Consume(ConsumeContext<ByEndpointNameCommand> context)
            {
                await context.Publish(new ByEndpointNameEvent());
            }
        }


        class ByEndpointNameConsumerDefinition :
            ConsumerDefinition<ByEndpointNameConsumer>
        {
            public ByEndpointNameConsumerDefinition()
            {
                EndpointName = "by_endpoint_name";
            }
        }


        class ByEndpointNameCommand
        {
        }


        class ByEndpointNameEvent
        {
        }
    }
}
