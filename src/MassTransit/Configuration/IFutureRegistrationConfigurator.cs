namespace MassTransit
{
    using System;
    using Automatonymous;
    using Futures;
    using Registration;


    public interface IFutureRegistrationConfigurator<TFuture> :
        IFutureRegistrationConfigurator
        where TFuture : MassTransitStateMachine<FutureState>
    {
        new IFutureRegistrationConfigurator<TFuture> Endpoint(Action<IFutureEndpointRegistrationConfigurator> configure);
        IFutureRegistrationConfigurator<TFuture> Repository(Action<ISagaRepositoryRegistrationConfigurator<FutureState>> configure);
    }


    public interface IFutureRegistrationConfigurator
    {
        IFutureRegistrationConfigurator Endpoint(Action<IFutureEndpointRegistrationConfigurator> configure);
    }
}
