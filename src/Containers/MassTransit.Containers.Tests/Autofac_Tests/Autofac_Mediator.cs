namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;


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
}
