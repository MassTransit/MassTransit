namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using Common_Tests;
    using Lamar;
    using NUnit.Framework;


    [TestFixture]
    public class Lamar_RequestClient_Context
        : RequestClient_Context
    {
        readonly IContainer _container;

        public Lamar_RequestClient_Context()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(x =>
                {
                    x.AddConsumer<InitialConsumer>();
                    x.AddConsumer<SubsequentConsumer>();

                    x.AddBus(context => BusControl);

                    x.AddRequestClient<InitialRequest>(InputQueueAddress);
                    x.AddRequestClient<SubsequentRequest>(SubsequentQueueAddress);
                });

                registry.For<IConsumeMessageObserver<InitialRequest>>().Use(context => GetConsumeObserver<InitialRequest>());
            });
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.GetInstance<IRequestClient<InitialRequest>>();
        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
    }
}
