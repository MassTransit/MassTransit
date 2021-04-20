namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using Common_Tests;
    using Mediator;
    using NUnit.Framework;
    using Registration;
    using Saga;


    [TestFixture]
    public class Autofac_Mediator :
        Common_Mediator
    {
        readonly IContainer _container;

        public Autofac_Mediator()
        {
            _container = new ContainerBuilder()
                .AddMediator(ConfigureRegistration)
                .Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IMediator Mediator => _container.Resolve<IMediator>();
    }


    [TestFixture]
    public class Autofac_Mediator_Bus :
        Common_Mediator_Bus
    {
        readonly IContainer _container;

        public Autofac_Mediator_Bus()
        {
            _container = new ContainerBuilder()
                .AddMediator(ConfigureRegistration)
                .AddMassTransit(x => x.AddBus(provider => BusControl))
                .Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IMediator Mediator => _container.Resolve<IMediator>();
        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Autofac_Mediator_Request :
        Common_Mediator_Request
    {
        readonly IContainer _container;

        public Autofac_Mediator_Request()
        {
            _container = new ContainerBuilder()
                .AddMediator(ConfigureRegistration)
                .Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IRequestClient<T> GetRequestClient<T>()
            where T : class
        {
            return _container.Resolve<IRequestClient<T>>();
        }
    }


    [TestFixture]
    public class Autofac_Mediator_Request_Filter :
        Common_Mediator_Request_Filter
    {
        readonly IContainer _container;

        public Autofac_Mediator_Request_Filter()
        {
            _container = new ContainerBuilder()
                .AddMediator(ConfigureRegistration)
                .Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IRequestClient<T> GetRequestClient<T>()
            where T : class
        {
            return _container.Resolve<IRequestClient<T>>();
        }

        protected override void ConfigureFilters(IMediatorRegistrationContext context, IMediatorConfigurator configurator)
        {
            AutofacFilterExtensions.UseSendFilter(configurator, typeof(PongFilter<>), context);
        }
    }


    [TestFixture]
    public class Autofac_Mediator_Saga :
        Common_Mediator_Saga
    {
        readonly IContainer _container;

        public Autofac_Mediator_Saga()
        {
            _container = new ContainerBuilder()
                .AddMediator(ConfigureRegistration)
                .Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IMediator Mediator => _container.Resolve<IMediator>();

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.Resolve<ISagaRepository<T>>();
        }
    }


    [TestFixture]
    public class Autofac_Mediator_FilterScope :
        Common_Mediator_FilterScope
    {
        readonly IContainer _container;

        public Autofac_Mediator_FilterScope()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ScopedContext>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ScopedConsumeFilter<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ScopedSendFilter<>)).InstancePerLifetimeScope();
            builder.RegisterInstance(EasyASource);
            builder.RegisterInstance(EasyBSource);
            builder.RegisterInstance(ScopedContextSource);
            builder.RegisterInstance(AsyncTestHarness);
            builder.AddMediator(ConfigureRegistration);
            _container = builder.Build();
        }

        protected override IMediator Mediator => _container.Resolve<IMediator>();

        protected override void ConfigureFilters(IMediatorRegistrationContext context, IMediatorConfigurator configurator)
        {
            AutofacFilterExtensions.UseConsumeFilter(configurator, typeof(ScopedConsumeFilter<>), context);
            AutofacFilterExtensions.UseSendFilter(configurator, typeof(ScopedSendFilter<>), context);
        }
    }
}
