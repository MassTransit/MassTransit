namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using Autofac;
    using Common_Tests;
    using Mediator;
    using NUnit.Framework;
    using Saga;


    [TestFixture]
    public class Autofac_Mediator :
        Common_Mediator
    {
        readonly IContainer _container;

        public Autofac_Mediator()
        {
            _container = new ContainerBuilder()
                .AddMassTransit(ConfigureRegistration)
                .Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IMediator Mediator => _container.Resolve<IMediator>();
    }


    [TestFixture]
    public class Autofac_Mediator_Request :
        Common_Mediator_Request
    {
        readonly IContainer _container;

        public Autofac_Mediator_Request()
        {
            _container = new ContainerBuilder()
                .AddMassTransit(ConfigureRegistration)
                .Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IRequestClient<T> GetRequestClient<T>()
            where T : class =>
            _container.Resolve<IRequestClient<T>>();
    }


    [TestFixture]
    public class Autofac_Mediator_Saga :
        Common_Mediator_Saga
    {
        readonly IContainer _container;

        public Autofac_Mediator_Saga()
        {
            _container = new ContainerBuilder()
                .AddMassTransit(ConfigureRegistration)
                .Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IMediator Mediator => _container.Resolve<IMediator>();

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.Resolve<ISagaRepository<T>>();
        }
    }
}
