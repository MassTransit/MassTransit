namespace MassTransit.DependencyInjection.Registration
{
    using Futures;


    public class DefaultFutureDefinition<TFuture> :
        FutureDefinition<TFuture>
        where TFuture : class, SagaStateMachine<FutureState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator)
        {
            endpointConfigurator.UseDelayedRedelivery(r => r.Intervals(5000, 30000, 120000));
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
