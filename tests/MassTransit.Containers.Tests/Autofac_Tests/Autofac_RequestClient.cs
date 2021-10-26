namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;


    [TestFixture]
    public class Autofac_RequestClient_Context
        : Common_RequestClient_Context
    {
        readonly IContainer _container;

        public Autofac_RequestClient_Context()
        {
            var builder = new ContainerBuilder();

            builder.AddMassTransit(ConfigureRegistration);

            builder.Register(context => GetConsumeObserver<InitialRequest>())
                .As<IConsumeMessageObserver<InitialRequest>>()
                .AsSelf()
                .SingleInstance();

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.Resolve<IRequestClient<InitialRequest>>();

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Autofac_ScopedClientFactory
        : Common_ScopedClientFactory
    {
        readonly IContainer _container;

        public Autofac_ScopedClientFactory()
        {
            var builder = new ContainerBuilder();

            builder.AddMassTransit(ConfigureRegistration);

            builder.Register(context => GetConsumeObserver<InitialRequest>())
                .As<IConsumeMessageObserver<InitialRequest>>()
                .AsSelf()
                .SingleInstance();

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.Resolve<IRequestClient<InitialRequest>>();

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Autofac_RequestClient_Generic
        : Common_RequestClient_Generic
    {
        readonly IContainer _container;

        public Autofac_RequestClient_Generic()
        {
            var builder = new ContainerBuilder();

            builder.AddMassTransit(ConfigureRegistration);
            builder.RegisterGenericRequestClient();

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.CreateRequestClient<InitialRequest>();

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Autofac_RequestClient_Outbox
        : Common_RequestClient_Outbox
    {
        readonly IContainer _container;

        public Autofac_RequestClient_Outbox()
        {
            var builder = new ContainerBuilder();

            builder.AddMassTransit(ConfigureRegistration);

            builder.Register(context => GetConsumeObserver<InitialRequest>())
                .As<IConsumeMessageObserver<InitialRequest>>()
                .AsSelf()
                .SingleInstance();

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.Resolve<IRequestClient<InitialRequest>>();

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Autofac_RequestClient_Outbox_Courier
        : Common_RequestClient_Outbox_Courier
    {
        readonly IContainer _container;

        public Autofac_RequestClient_Outbox_Courier()
        {
            var builder = new ContainerBuilder();

            builder.AddMassTransit(ConfigureRegistration);

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }
}
