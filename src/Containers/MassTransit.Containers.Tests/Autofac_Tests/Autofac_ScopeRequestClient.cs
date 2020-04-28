namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework.Messages;


    [TestFixture]
    public class Autofac_ScopeRequestClient :
        Common_ScopeRequestClient<ILifetimeScope>
    {
        readonly IContainer _container;
        readonly ILifetimeScope _childContainer;

        public Autofac_ScopeRequestClient()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddRequestClient<SimpleMessageClass>(InputQueueAddress);
                x.AddRequestClient<PingMessage>();
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

        protected override IRequestClient<SimpleMessageClass> GetSendRequestClient()
        {
            return _childContainer.Resolve<IRequestClient<SimpleMessageClass>>();
        }

        protected override IRequestClient<PingMessage> GetPublishRequestClient()
        {
            return _childContainer.Resolve<IRequestClient<PingMessage>>();
        }

        protected override void AssertScopesAreEqual(ILifetimeScope actual)
        {
            Assert.AreEqual(_childContainer, actual);
        }
    }
}
