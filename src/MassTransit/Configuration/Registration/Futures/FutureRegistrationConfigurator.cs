namespace MassTransit.Registration
{
    using System;
    using Automatonymous;
    using MassTransit.Futures;


    public class FutureRegistrationConfigurator<TFuture> :
        IFutureRegistrationConfigurator<TFuture>
        where TFuture : MassTransitStateMachine<FutureState>
    {
        readonly IRegistrationConfigurator _configurator;

        public FutureRegistrationConfigurator(IRegistrationConfigurator configurator)
        {
            _configurator = configurator;
        }

        public IFutureRegistrationConfigurator<TFuture> Endpoint(Action<IFutureEndpointRegistrationConfigurator<TFuture>> configure)
        {
            var configurator = new FutureEndpointRegistrationConfigurator<TFuture>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<FutureEndpointDefinition<TFuture>, TFuture>(configurator.Settings);

            return this;
        }
    }
}
