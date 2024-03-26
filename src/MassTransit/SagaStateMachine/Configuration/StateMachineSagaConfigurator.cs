#nullable enable
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Configuration;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        internal class StateMachineSagaConfigurator :
            ISagaConfigurator<TInstance>,
            IReceiveEndpointSpecification
        {
            readonly StateMachineConnector _connector;
            readonly ISagaRepository<TInstance> _repository;
            readonly ISagaSpecification<TInstance> _specification;

            public StateMachineSagaConfigurator(SagaStateMachine<TInstance> stateMachine, ISagaRepository<TInstance> repository,
                ISagaConfigurationObserver observer)
            {
                _repository = repository;

                _connector = new StateMachineConnector(stateMachine);

                _specification = _connector.CreateSagaSpecification<TInstance>();

                _specification.ConnectSagaConfigurationObserver(observer);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }

            public void Configure(IReceiveEndpointBuilder builder)
            {
                _connector.ConnectSaga(builder, _repository, _specification);
            }

            public int? ConcurrentMessageLimit
            {
                set => _specification.ConcurrentMessageLimit = value;
            }

            public void Message<T>(Action<ISagaMessageConfigurator<T>> configure)
                where T : class
            {
                _specification.Message(configure);
            }

            public void SagaMessage<T>(Action<ISagaMessageConfigurator<TInstance, T>> configure)
                where T : class
            {
                _specification.SagaMessage(configure);
            }

            public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TInstance>> specification)
            {
                _specification.AddPipeSpecification(specification);
            }

            public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
            {
                return _specification.ConnectSagaConfigurationObserver(observer);
            }

            public T Options<T>(Action<T>? configure)
                where T : IOptions, new()
            {
                return _specification.Options(configure);
            }

            public T Options<T>(T options, Action<T>? configure)
                where T : IOptions
            {
                return _specification.Options(options, configure);
            }

            public bool TryGetOptions<T>(out T options)
                where T : IOptions
            {
                return _specification.TryGetOptions(out options);
            }

            public IEnumerable<T> SelectOptions<T>()
                where T : class
            {
                return _specification.SelectOptions<T>();
            }
        }
    }
}
