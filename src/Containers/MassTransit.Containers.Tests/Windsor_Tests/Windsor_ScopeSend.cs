namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.MicroKernel;
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;
    using Scoping;


    [TestFixture]
    public class Windsor_ScopeSend :
        Common_ScopeSend<IKernel>
    {
        readonly IWindsorContainer _container;

        public Windsor_ScopeSend()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddBus(provider => BusControl);
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override ISendScopeProvider GetSendScopeProvider()
        {
            return _container.Resolve<ISendScopeProvider>();
        }
    }
}
