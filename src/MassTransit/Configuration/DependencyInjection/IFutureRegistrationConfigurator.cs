namespace MassTransit
{
    using System;


    public interface IFutureRegistrationConfigurator<TFuture> :
        IFutureRegistrationConfigurator
        where TFuture : class, SagaStateMachine<FutureState>
    {
        new IFutureRegistrationConfigurator<TFuture> Endpoint(Action<IEndpointRegistrationConfigurator> configure);
        IFutureRegistrationConfigurator<TFuture> Repository(Action<ISagaRepositoryRegistrationConfigurator<FutureState>> configure);
    }


    public interface IFutureRegistrationConfigurator
    {
        IFutureRegistrationConfigurator Endpoint(Action<IEndpointRegistrationConfigurator> configure);
    }
}
