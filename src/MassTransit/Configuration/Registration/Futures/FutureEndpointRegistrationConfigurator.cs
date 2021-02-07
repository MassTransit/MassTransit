namespace MassTransit.Registration
{
    using Automatonymous;
    using MassTransit.Futures;


    public class FutureEndpointRegistrationConfigurator<TFuture> :
        EndpointRegistrationConfigurator<TFuture>,
        IFutureEndpointRegistrationConfigurator<TFuture>
        where TFuture : MassTransitStateMachine<FutureState>
    {
    }
}
