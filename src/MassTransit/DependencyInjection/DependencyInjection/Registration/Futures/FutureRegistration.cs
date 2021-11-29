namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;
    using Futures;
    using Microsoft.Extensions.DependencyInjection;
    using Transports;


    public class FutureRegistration<TFuture> :
        IFutureRegistration
        where TFuture : class, SagaStateMachine<FutureState>
    {
        IFutureDefinition<TFuture> _definition;

        public Type Type => typeof(TFuture);

        public void Configure(IReceiveEndpointConfigurator configurator, IServiceProvider provider)
        {
            var stateMachine = provider.GetRequiredService<TFuture>();
            var repository = provider.GetRequiredService<ISagaRepository<FutureState>>();

            var decoratorRegistration = provider.GetService<ISagaRepositoryDecoratorRegistration<FutureState>>();
            if (decoratorRegistration != null)
                repository = decoratorRegistration.DecorateSagaRepository(repository);

            var sagaConfigurator = new StateMachineSagaConfigurator<FutureState>(stateMachine, repository, configurator);

            GetFutureDefinition(provider)
                .Configure(configurator, sagaConfigurator);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Future: {FutureType}",
                configurator.InputAddress.GetEndpointName(), TypeCache<TFuture>.ShortName);

            configurator.AddEndpointSpecification(sagaConfigurator);
        }

        public IFutureDefinition GetDefinition(IServiceProvider provider)
        {
            return GetFutureDefinition(provider);
        }

        IFutureDefinition<TFuture> GetFutureDefinition(IServiceProvider provider)
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
