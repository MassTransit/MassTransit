namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System;
    using Common_Tests;
    using GreenPipes;
    using NUnit.Framework;
    using Scenarios;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    [TestFixture]
    public class SimpleInjector_Consumer :
        Common_Consumer
    {
        readonly Container _container;

        public SimpleInjector_Consumer()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<SimpleConsumer>();
                cfg.AddBus(() => BusControl);
            });

            _container.Register<ISimpleConsumerDependency, SimpleConsumerDependency>(Lifestyle.Scoped);

            _container.Register<AnotherMessageConsumer, AnotherMessageConsumerImpl>(Lifestyle.Scoped);
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureConsumer(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimpleConsumer>(_container);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseExecute(context => Console.WriteLine(
                $"Received (input_queue): {context.ReceiveContext.TransportHeaders.Get("MessageId", "N/A")}, Types = ({string.Join(",", context.SupportedMessageTypes)})"));

            base.ConfigureInMemoryBus(configurator);
        }
    }


    [TestFixture]
    public class SimpleInjector_Consumer_Endpoint :
        Common_Consumer_Endpoint
    {
        readonly Container _container;

        public SimpleInjector_Consumer_Endpoint()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<SimplerConsumer>()
                    .Endpoint(e => e.Name = "custom-endpoint-name");

                cfg.AddBus(() => BusControl);
            });
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureEndpoints(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(_container);
        }
    }
}
