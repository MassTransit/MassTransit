namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    [TestFixture]
    public class SimpleInjector_RequestClient_Context
        : RequestClient_Context
    {
        readonly Container _container;

        public SimpleInjector_RequestClient_Context()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

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

        protected override void ConfigureInitialConsumer(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<InitialConsumer>(_container);
        }

        protected override void ConfigureSubsequentConsumer(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SubsequentConsumer>(_container);
        }
    }
}
