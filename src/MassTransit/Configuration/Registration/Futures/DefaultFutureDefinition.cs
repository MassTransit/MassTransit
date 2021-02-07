namespace MassTransit.Registration
{
    using Automatonymous;
    using Conductor;
    using Conductor.Directory;
    using Futures;
    using GreenPipes;


    public class DefaultFutureDefinition<TFuture> :
        FutureDefinition<TFuture>,
        IConfigureServiceDirectory
        where TFuture : MassTransitStateMachine<FutureState>
    {
        public void Configure(IServiceDirectoryConfigurator configurator)
        {
            configurator.AddFuture(typeof(TFuture));
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator)
        {
            endpointConfigurator.UseDelayedRedelivery(r => r.Intervals(5000, 30000, 120000));
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
