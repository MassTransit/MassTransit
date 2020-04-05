namespace MassTransit.SagaConfigurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using Saga;
    using Saga.Connectors;


    public class SagaConfigurator<TSaga> :
        ISagaConfigurator<TSaga>,
        IReceiveEndpointSpecification
        where TSaga : class, ISaga
    {
        readonly ISagaRepository<TSaga> _sagaRepository;
        readonly ISagaSpecification<TSaga> _specification;

        public SagaConfigurator(ISagaRepository<TSaga> sagaRepository, ISagaConfigurationObserver observer)
        {
            _sagaRepository = sagaRepository;

            _specification = SagaConnectorCache<TSaga>.Connector.CreateSagaSpecification<TSaga>();

            _specification.ConnectSagaConfigurationObserver(observer);
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            SagaConnectorCache<TSaga>.Connector.ConnectSaga(builder, _sagaRepository, _specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_sagaRepository == null)
                yield return this.Failure("The saga repository cannot be null. How else are we going to save stuff? #facetopalm");

            foreach (var result in _specification.Validate())
                yield return result;
        }

        public void Message<T>(Action<ISagaMessageConfigurator<T>> configure)
            where T : class
        {
            _specification.Message(configure);
        }

        public void SagaMessage<T>(Action<ISagaMessageConfigurator<TSaga, T>> configure)
            where T : class
        {
            _specification.SagaMessage(configure);
        }

        public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TSaga>> specification)
        {
            _specification.AddPipeSpecification(specification);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _specification.ConnectSagaConfigurationObserver(observer);
        }
    }
}
