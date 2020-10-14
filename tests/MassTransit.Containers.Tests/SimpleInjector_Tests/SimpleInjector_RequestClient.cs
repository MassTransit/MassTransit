namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using SimpleInjector;


    [TestFixture]
    public class SimpleInjector_RequestClient_Context
        : RequestClient_Context
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        readonly Container _container;

        public SimpleInjector_RequestClient_Context()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();

            _container.AddMassTransit(x =>
            {
                x.AddConsumer<InitialConsumer>();
                x.AddConsumer<SubsequentConsumer>();

                x.AddBus(() => BusControl);

                x.AddRequestClient<InitialRequest>(InputQueueAddress);
                x.AddRequestClient<SubsequentRequest>(SubsequentQueueAddress);
            });

            _container.Register<IConsumeMessageObserver<InitialRequest>>(GetConsumeObserver<InitialRequest>);
        }

        protected override IRequestClient<InitialRequest> RequestClient => _container.GetInstance<IRequestClient<InitialRequest>>();

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }
}
