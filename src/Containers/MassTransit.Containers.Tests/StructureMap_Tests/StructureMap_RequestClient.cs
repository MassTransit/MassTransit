namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using StructureMap;


    [TestFixture]
    public class StructureMap_RequestClient_Context
        : RequestClient_Context
    {
        readonly IContainer _container;

        public StructureMap_RequestClient_Context()
        {
            _container = new Container(collection =>
            {
                collection.AddMassTransit(x =>
                {
                    x.AddConsumer<InitialConsumer>();
                    x.AddConsumer<SubsequentConsumer>();

                    x.AddBus(context => BusControl);

                    x.AddRequestClient<InitialRequest>(InputQueueAddress);
                    x.AddRequestClient<SubsequentRequest>(SubsequentQueueAddress);
                });

                collection.For<IConsumeMessageObserver<InitialRequest>>().Use(context => GetConsumeObserver<InitialRequest>());
            });
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.GetInstance<IRequestClient<InitialRequest>>();
        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
    }
}
