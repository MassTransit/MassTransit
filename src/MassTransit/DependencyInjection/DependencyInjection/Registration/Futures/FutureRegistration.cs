namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Transports;


    public class FutureRegistration<TFuture> :
        IFutureRegistration
        where TFuture : class, SagaStateMachine<FutureState>
    {
        readonly IContainerSelector _selector;
        IFutureDefinition<TFuture> _definition;

        public FutureRegistration(IContainerSelector selector)
        {
            _selector = selector;
            IncludeInConfigureEndpoints = !Type.HasAttribute<ExcludeFromConfigureEndpointsAttribute>();
        }

        public Type Type => typeof(TFuture);

        public bool IncludeInConfigureEndpoints { get; set; }

        public void Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
        {
            var stateMachine = context.GetRequiredService<TFuture>();
            ISagaRepository<FutureState> repository = new DependencyInjectionSagaRepository<FutureState>(context);

            var decoratorRegistration = context.GetService<ISagaRepositoryDecoratorRegistration<FutureState>>();
            if (decoratorRegistration != null)
                repository = decoratorRegistration.DecorateSagaRepository(repository);

            var sagaConfigurator = new MassTransitStateMachine<FutureState>.StateMachineSagaConfigurator(stateMachine, repository, configurator);

            GetFutureDefinition(context)
                .Configure(configurator, sagaConfigurator, context);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Future: {FutureType}",
                configurator.InputAddress.GetEndpointName(), TypeCache<TFuture>.ShortName);

            configurator.AddEndpointSpecification(sagaConfigurator);
        }

        public IFutureDefinition GetDefinition(IRegistrationContext context)
        {
            return GetFutureDefinition(context);
        }

        IFutureDefinition<TFuture> GetFutureDefinition(IServiceProvider provider)
        {
            if (_definition != null)
                return _definition;

            _definition = _selector.GetDefinition<IFutureDefinition<TFuture>>(provider) ?? new DefaultFutureDefinition<TFuture>();

            IEndpointDefinition<TFuture> endpointDefinition = _selector.GetEndpointDefinition<TFuture>(provider);
            if (endpointDefinition != null)
                _definition.EndpointDefinition = endpointDefinition;

            return _definition;
        }
    }
}
