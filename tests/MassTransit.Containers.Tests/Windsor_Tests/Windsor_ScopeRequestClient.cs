namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Lifestyle;
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework.Messages;


    [TestFixture]
    public class Windsor_ScopeRequestClient :
        Common_ScopeRequestClient<IKernel>
    {
        readonly IWindsorContainer _container;
        readonly IDisposable _childContainer;

        public Windsor_ScopeRequestClient()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddRequestClient<SimpleMessageClass>(InputQueueAddress);
                x.AddRequestClient<PingMessage>();
                x.AddBus(provider => BusControl);
            });
            _childContainer = _container.BeginScope();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _childContainer.Dispose();
            _container.Dispose();
        }

        protected override IRequestClient<SimpleMessageClass> GetSendRequestClient()
        {
            return _container.Resolve<IRequestClient<SimpleMessageClass>>();
        }

        protected override IRequestClient<PingMessage> GetPublishRequestClient()
        {
            return _container.Resolve<IRequestClient<PingMessage>>();
        }

        protected override void AssertScopesAreEqual(IKernel actual)
        {
            Assert.AreEqual(_container.Kernel, actual);
        }
    }
}
