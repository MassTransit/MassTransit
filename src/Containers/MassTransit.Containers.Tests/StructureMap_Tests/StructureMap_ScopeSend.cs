namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using Scoping;
    using StructureMap;


    [TestFixture]
    public class StructureMap_ScopeSend :
        Common_ScopeSend<IContainer>
    {
        readonly IContainer _container;

        public StructureMap_ScopeSend()
        {
            _container = new Container(expression =>
            {
                expression.AddMassTransit(cfg =>
                {
                    cfg.AddBus(context => BusControl);
                });
            });
        }

        protected override ISendScopeProvider GetSendScopeProvider()
        {
            return _container.GetInstance<ISendScopeProvider>();
        }
    }
}
