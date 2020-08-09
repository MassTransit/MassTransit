namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Common_Tests;
    using GreenPipes;
    using NUnit.Framework;
    using Scenarios;


    [TestFixture]
    public class Autofac_Consumer :
        Common_Consumer
    {
        readonly IContainer _container;

        public Autofac_Consumer()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddConsumer<SimpleConsumer>();

                x.AddBus(provider => BusControl);
            });

            builder.RegisterType<SimpleConsumerDependency>()
                .As<ISimpleConsumerDependency>();

            builder.RegisterType<AnotherMessageConsumerImpl>()
                .As<AnotherMessageConsumer>();

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Autofac_Consumer_Endpoint :
        Common_Consumer_Endpoint
    {
        readonly IContainer _container;

        public Autofac_Consumer_Endpoint()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddConsumer<SimplerConsumer>()
                    .Endpoint(e => e.Name = "custom-endpoint-name");

                x.AddBus(provider => BusControl);
            });

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Autofac_Consumer_ServiceEndpoint :
        Common_Consumer_ServiceEndpoint
    {
        readonly IContainer _container;

        public Autofac_Consumer_ServiceEndpoint()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddConsumer<PingRequestConsumer>();

                x.AddBus(provider => BusControl);
            });

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Autofac_Consume_Filter :
        Common_Consume_Filter
    {
        readonly IContainer _container;

        public Autofac_Consume_Filter()
        {
            var builder = new ContainerBuilder();
            builder.Register(_ => new MyId(Guid.NewGuid())).InstancePerLifetimeScope();
            builder.RegisterInstance(TaskCompletionSource);
            builder.AddMassTransit(ConfigureRegistration);
            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override void ConfigureFilter(IConsumePipeConfigurator configurator)
        {
            AutofacFilterExtensions.UseConsumeFilter(configurator, typeof(ScopedFilter<>), Registration);
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Autofac_Consume_FilterScope :
        Common_Consume_FilterScope
    {
        readonly IContainer _container;

        public Autofac_Consume_FilterScope()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ScopedContext>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ScopedConsumeFilter<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ScopedSendFilter<>)).InstancePerLifetimeScope();
            builder.RegisterInstance(EasyASource);
            builder.RegisterInstance(EasyBSource);
            builder.RegisterInstance(ScopedContextSource);
            builder.RegisterInstance(AsyncTestHarness);
            builder.AddMassTransit(ConfigureRegistration);
            _container = builder.Build();
        }

        protected override void ConfigureFilters(IInMemoryReceiveEndpointConfigurator configurator)
        {
            AutofacFilterExtensions.UseConsumeFilter(configurator, typeof(ScopedConsumeFilter<>), Registration);
            AutofacFilterExtensions.UseSendFilter(configurator, typeof(ScopedSendFilter<>), Registration);
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Autofac_Consumer_FilterOrder :
        Common_Consumer_FilterOrder
    {
        readonly IContainer _container;

        public Autofac_Consumer_FilterOrder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(MessageCompletion);
            builder.RegisterInstance(ConsumerCompletion);
            builder.RegisterInstance(ConsumerMessageCompletion);
            builder.RegisterType<MessageFilter<EasyMessage, ILifetimeScope>>().As<IFilter<ConsumeContext<EasyMessage>>>();
            builder.RegisterType<ConsumerFilter<EasyConsumer, ILifetimeScope>>().As<IFilter<ConsumerConsumeContext<EasyConsumer>>>();
            builder.RegisterType<ConsumerMessageFilter<EasyConsumer, EasyMessage, ILifetimeScope>>()
                .As<IFilter<ConsumerConsumeContext<EasyConsumer, EasyMessage>>>();
            builder.AddMassTransit(ConfigureRegistration);

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();

        protected override IFilter<ConsumerConsumeContext<EasyConsumer, EasyMessage>> CreateConsumerMessageFilter()
        {
            return _container.Resolve<IFilter<ConsumerConsumeContext<EasyConsumer, EasyMessage>>>();
        }

        protected override IFilter<ConsumerConsumeContext<EasyConsumer>> CreateConsumerFilter()
        {
            return _container.Resolve<IFilter<ConsumerConsumeContext<EasyConsumer>>>();
        }

        protected override IFilter<ConsumeContext<EasyMessage>> CreateMessageFilter()
        {
            return _container.Resolve<IFilter<ConsumeContext<EasyMessage>>>();
        }
    }
}
