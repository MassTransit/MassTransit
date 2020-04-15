namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;
    using Scoping;


    [TestFixture]
    public class Autofac_ScopeSend :
        Common_ScopeSend<ILifetimeScope>
    {
        readonly IContainer _container;

        public Autofac_ScopeSend()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddBus(provider => BusControl);
            });

            _container = builder.Build();
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
