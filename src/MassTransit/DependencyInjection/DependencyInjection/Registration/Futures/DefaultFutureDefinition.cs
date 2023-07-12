namespace MassTransit.DependencyInjection.Registration
{
    public class DefaultFutureDefinition<TFuture> :
        FutureDefinition<TFuture>
        where TFuture : class, SagaStateMachine<FutureState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator,
            IRegistrationContext context)
        {
            endpointConfigurator.UseDelayedRedelivery(r => r.Intervals(5000, 30000, 120000));
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500));
            endpointConfigurator.UseInMemoryOutbox(context);
        }
    }
}
