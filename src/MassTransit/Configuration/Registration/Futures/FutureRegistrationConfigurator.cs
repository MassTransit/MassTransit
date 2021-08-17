namespace MassTransit.Registration
{
    using System;
    using Automatonymous;
    using Futures;


    public class FutureRegistrationConfigurator<TFuture> :
        IFutureRegistrationConfigurator<TFuture>
        where TFuture : MassTransitStateMachine<FutureState>
    {
        readonly IRegistrationConfigurator _configurator;
        readonly IContainerRegistrar _registrar;

        public FutureRegistrationConfigurator(IRegistrationConfigurator configurator, IContainerRegistrar registrar)
        {
            _configurator = configurator;
            _registrar = registrar;
        }

        IFutureRegistrationConfigurator IFutureRegistrationConfigurator.Endpoint(Action<IFutureEndpointRegistrationConfigurator> configure)
        {
            return Endpoint(configure);
        }

        public IFutureRegistrationConfigurator<TFuture> Endpoint(Action<IFutureEndpointRegistrationConfigurator> configure)
        {
            var configurator = new FutureEndpointRegistrationConfigurator<TFuture>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<FutureEndpointDefinition<TFuture>, TFuture>(configurator.Settings);

            return this;
        }

        public IFutureRegistrationConfigurator<TFuture> Repository(Action<ISagaRepositoryRegistrationConfigurator<FutureState>> configure)
        {
            var configurator = new SagaRepositoryRegistrationConfigurator<FutureState>(_registrar);

            configure?.Invoke(configurator);

            return this;
        }
    }
}
