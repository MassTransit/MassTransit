namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;
    using Futures;


    public class FutureRegistrationConfigurator<TFuture> :
        IFutureRegistrationConfigurator<TFuture>
        where TFuture : class, SagaStateMachine<FutureState>
    {
        readonly IRegistrationConfigurator _configurator;

        public FutureRegistrationConfigurator(IRegistrationConfigurator configurator)
        {
            _configurator = configurator;
        }

        IFutureRegistrationConfigurator IFutureRegistrationConfigurator.Endpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            return Endpoint(configure);
        }

        public IFutureRegistrationConfigurator<TFuture> Endpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            var configurator = new EndpointRegistrationConfigurator<TFuture>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<FutureEndpointDefinition<TFuture>, TFuture>(configurator.Settings);

            return this;
        }

        public IFutureRegistrationConfigurator<TFuture> Repository(Action<ISagaRepositoryRegistrationConfigurator<FutureState>> configure)
        {
            var configurator = new SagaRepositoryRegistrationConfigurator<FutureState>(_configurator);

            configure?.Invoke(configurator);

            return this;
        }
    }
}
