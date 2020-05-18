namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using Common_Tests;
    using Mediator;
    using NUnit.Framework;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    [TestFixture]
    public class SimpleInjector_Mediator :
        Common_Mediator
    {
        readonly Container _container;

        public SimpleInjector_Mediator()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            _container.AddMassTransit(ConfigureRegistration);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IMediator Mediator => _container.GetInstance<IMediator>();
    }
}
