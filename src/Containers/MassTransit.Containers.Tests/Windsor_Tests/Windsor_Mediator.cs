namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.Windsor;
    using Common_Tests;
    using Mediator;
    using NUnit.Framework;


    [TestFixture]
    public class Windsor_Mediator :
        Common_Mediator
    {
        readonly IWindsorContainer _container;

        public Windsor_Mediator()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(ConfigureRegistration);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IMediator Mediator => _container.Resolve<IMediator>();
    }
}
