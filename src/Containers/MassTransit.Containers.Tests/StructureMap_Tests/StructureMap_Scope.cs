namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using StructureMap;


    [TestFixture]
    public class StructureMap_Scope :
        Common_Scope
    {
        readonly IContainer _container;

        public StructureMap_Scope()
        {
            _container = new Container(expression =>
            {
                expression.AddMassTransit(cfg =>
                {
                    cfg.AddBus(context => BusControl);
                });
            });
        }

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            return _container.GetInstance<ISendEndpointProvider>();
        }

        protected override IPublishEndpoint GetPublishEndpoint()
        {
            return _container.GetInstance<IPublishEndpoint>();
        }
    }
}