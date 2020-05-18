namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using Common_Tests;
    using Mediator;
    using NUnit.Framework;
    using StructureMap;


    [TestFixture]
    public class StructureMap_Mediator :
        Common_Mediator
    {
        readonly IContainer _container;

        public StructureMap_Mediator()
        {
            _container = new Container(expression => expression.AddMassTransit(ConfigureRegistration));
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IMediator Mediator => _container.GetInstance<IMediator>();
    }
}
