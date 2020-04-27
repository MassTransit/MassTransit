namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;


    [TestFixture]
    public class Autofac_ScopePublish :
        Common_ScopePublish<ILifetimeScope>
    {
        readonly IContainer _container;
        readonly ILifetimeScope _childContainer;

        public Autofac_ScopePublish()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddBus(provider => BusControl);
            });

            _container = builder.Build();
            _childContainer = _container.BeginLifetimeScope();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _childContainer.DisposeAsync();
            await _container.DisposeAsync();
        }

        protected override IPublishEndpoint GetPublishEndpoint()
        {
            return _childContainer.Resolve<IPublishEndpoint>();
        }

        protected override void AssertScopesAreEqual(ILifetimeScope actual)
        {
            Assert.AreEqual(_childContainer, actual);
        }
    }
}
