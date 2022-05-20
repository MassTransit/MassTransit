#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


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
            return _specification.Validate();
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
