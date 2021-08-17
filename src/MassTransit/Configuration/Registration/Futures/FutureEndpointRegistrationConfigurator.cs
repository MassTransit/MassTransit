namespace MassTransit.Registration
{
    using Automatonymous;
    using Futures;


    public class FutureEndpointRegistrationConfigurator<TFuture> :
        EndpointRegistrationConfigurator<TFuture>,
        IFutureEndpointRegistrationConfigurator
        where TFuture : MassTransitStateMachine<FutureState>
    {
    }
}
