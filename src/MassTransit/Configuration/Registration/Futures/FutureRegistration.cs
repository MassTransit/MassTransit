namespace MassTransit.Registration
{
    using System;
    using Automatonymous;
    using Automatonymous.SagaConfigurators;
    using Context;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Futures;
    using Saga;


    public class FutureRegistration<TFuture> :
        IFutureRegistration
        where TFuture : MassTransitStateMachine<FutureState>
    {
        IFutureDefinition<TFuture> _definition;

        public Type FutureType => typeof(TFuture);

        public void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var stateMachine = configurationServiceProvider.GetRequiredService<TFuture>();
            var repository = configurationServiceProvider.GetRequiredService<ISagaRepository<FutureState>>();

            var decoratorRegistration = configurationServiceProvider.GetService<ISagaRepositoryDecoratorRegistration<FutureState>>();
            if (decoratorRegistration != null)
                repository = decoratorRegistration.DecorateSagaRepository(repository);

            var sagaConfigurator = new StateMachineSagaConfigurator<FutureState>(stateMachine, repository, configurator);

            GetFutureDefinition(configurationServiceProvider)
                .Configure(configurator, sagaConfigurator);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Future: {FutureType}",
                configurator.InputAddress.GetLastPart(), TypeCache<TFuture>.ShortName);

            configurator.AddEndpointSpecification(sagaConfigurator);
        }

        public IFutureDefinition GetDefinition(IConfigurationServiceProvider provider)
        {
            return GetFutureDefinition(provider);
        }

        IFutureDefinition<TFuture> GetFutureDefinition(IConfigurationServiceProvider provider)
        {
            if (_definition != null)
                return _definition;

            _definition = provider.GetService<IFutureDefinition<TFuture>>() ?? new DefaultFutureDefinition<TFuture>();

            var endpointDefinition = provider.GetService<IEndpointDefinition<TFuture>>();
            if (endpointDefinition != null)
                _definition.EndpointDefinition = endpointDefinition;

            return _definition;
        }
    }
}
