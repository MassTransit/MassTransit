namespace MassTransit
{
    using System;
    using Automatonymous;
    using Futures;


    public interface IFutureRegistrationConfigurator<TFuture>
        where TFuture : MassTransitStateMachine<FutureState>
    {
        IFutureRegistrationConfigurator<TFuture> Endpoint(Action<IFutureEndpointRegistrationConfigurator<TFuture>> configure);
    }
}
