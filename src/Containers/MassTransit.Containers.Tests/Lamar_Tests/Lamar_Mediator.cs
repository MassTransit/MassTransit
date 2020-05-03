namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using Common_Tests;
    using Lamar;
    using Mediator;
    using NUnit.Framework;


    [TestFixture]
    public class Lamar_Mediator :
        Common_Mediator
    {
        readonly IContainer _container;

        public Lamar_Mediator()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(ConfigureRegistration);
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IMediator Mediator => _container.GetInstance<IMediator>();
    }
}
