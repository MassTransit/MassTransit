namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Middleware;


    public class SagaSpecification<TSaga> :
        OptionsSet,
        ISagaSpecification<TSaga>
        where TSaga : class, ISaga
    {
        readonly ConnectHandle[] _handles;
        readonly IReadOnlyDictionary<Type, ISagaMessageSpecification<TSaga>> _messageTypes;
        protected readonly SagaConfigurationObservable Observers;
        IConcurrencyLimiter _concurrencyLimiter;

        public SagaSpecification(IEnumerable<ISagaMessageSpecification<TSaga>> messageSpecifications)
        {
            _messageTypes = messageSpecifications.ToDictionary(x => x.MessageType);

            Observers = new SagaConfigurationObservable();
            _handles = _messageTypes.Values.Select(x => x.ConnectSagaConfigurationObserver(Observers)).ToArray();
        }

        public int? ConcurrentMessageLimit { get; set; }

        public void Message<T>(Action<ISagaMessageConfigurator<T>> configure)
            where T : class
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            ISagaMessageSpecification<TSaga, T> specification = GetMessageSpecification<T>();

            configure(specification);
        }

        public void SagaMessage<T>(Action<ISagaMessageConfigurator<TSaga, T>> configure)
            where T : class
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            ISagaMessageSpecification<TSaga, T> specification = GetMessageSpecification<T>();

            configure(specification);
        }

        public ISagaMessageSpecification<TSaga, T> GetMessageSpecification<T>()
            where T : class
        {
            if (!_messageTypes.TryGetValue(typeof(T), out ISagaMessageSpecification<TSaga> specification))
                throw new ArgumentException($"MessageType {TypeCache<T>.ShortName} is not consumed by {TypeCache<TSaga>.ShortName}");

            return specification.GetMessageSpecification<T>();
        }

        public void ConfigureMessagePipe<T>(IPipeConfigurator<ConsumeContext<T>> pipeConfigurator)
            where T : class
        {
            if (ConcurrentMessageLimit.HasValue)
            {
                _concurrencyLimiter ??= new ConcurrencyLimiter(ConcurrentMessageLimit.Value, TypeCache<TSaga>.ShortName);

                pipeConfigurator.AddPipeSpecification(new ConcurrencyLimitConsumePipeSpecification<T>(_concurrencyLimiter));
            }
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            Observers.ForEach(observer => observer.SagaConfigured(this));

            foreach (var result in _messageTypes.Values.SelectMany(x => x.Validate()))
                yield return result;

            foreach (var result in ValidateOptions())
                yield return result;
        }

        public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TSaga>> specification)
        {
            foreach (ISagaMessageSpecification<TSaga> messageSpecification in _messageTypes.Values)
                messageSpecification.AddPipeSpecification(specification);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return Observers.Connect(observer);
        }
    }
}
