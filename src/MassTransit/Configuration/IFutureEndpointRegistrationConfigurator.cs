namespace MassTransit
{
    using Automatonymous;
    using Futures;
    using Registration;


    public interface IFutureEndpointRegistrationConfigurator<TFuture> :
        IEndpointRegistrationConfigurator
        where TFuture : MassTransitStateMachine<FutureState>
    {
    }
}
