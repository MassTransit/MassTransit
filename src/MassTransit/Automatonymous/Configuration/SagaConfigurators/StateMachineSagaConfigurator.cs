namespace Automatonymous.SagaConfigurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;
    using MassTransit.SagaConfigurators;
    using StateMachineConnectors;


    public class StateMachineSagaConfigurator<TInstance> :
        ISagaConfigurator<TInstance>,
        IReceiveEndpointSpecification
        where TInstance : class, SagaStateMachineInstance
    {
        readonly StateMachineConnector<TInstance> _connector;
        readonly ISagaRepository<TInstance> _repository;
        readonly ISagaSpecification<TInstance> _specification;
        readonly SagaStateMachine<TInstance> _stateMachine;

        public StateMachineSagaConfigurator(SagaStateMachine<TInstance> stateMachine, ISagaRepository<TInstance> repository,
            ISagaConfigurationObserver observer)
        {
            _stateMachine = stateMachine;
            _repository = repository;

            _connector = new StateMachineConnector<TInstance>(_stateMachine);

            _specification = _connector.CreateSagaSpecification<TInstance>();

            _specification.ConnectSagaConfigurationObserver(observer);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_stateMachine == null)
                yield return this.Failure("StateMachine", "must not be null");

            if (_repository == null)
                yield return this.Failure("Repository", "must not be null");

            foreach (var result in _specification.Validate())
                yield return result;
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            _connector.ConnectSaga(builder, _repository, _specification);
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
    }
}
