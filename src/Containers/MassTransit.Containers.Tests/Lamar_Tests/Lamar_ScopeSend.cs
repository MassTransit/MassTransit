namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using Common_Tests;
    using Lamar;
    using NUnit.Framework;
    using Scoping;


    [TestFixture]
    public class Lamar_ScopeSend :
        Common_ScopeSend<INestedContainer>
    {
        readonly IContainer _container;

        public Lamar_ScopeSend()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddBus(context => BusControl);
                });
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override ISendScopeProvider GetSendScopeProvider()
        {
            return _container.GetInstance<ISendScopeProvider>();
        }
    }
}
