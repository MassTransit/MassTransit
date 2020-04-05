namespace MassTransit.SagaSpecifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using Metadata;
    using Saga;
    using SagaConfigurators;


    public class SagaSpecification<TSaga> :
        ISagaSpecification<TSaga>
        where TSaga : class, ISaga
    {
        readonly ConnectHandle[] _handles;
        readonly IReadOnlyDictionary<Type, ISagaMessageSpecification<TSaga>> _messageTypes;
        protected readonly SagaConfigurationObservable Observers;

        public SagaSpecification(IEnumerable<ISagaMessageSpecification<TSaga>> messageSpecifications)
        {
            _messageTypes = messageSpecifications.ToDictionary(x => x.MessageType);

            Observers = new SagaConfigurationObservable();
            _handles = _messageTypes.Values.Select(x => x.ConnectSagaConfigurationObserver(Observers)).ToArray();
        }

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
            {
                throw new ArgumentException($"MessageType {TypeMetadataCache<T>.ShortName} is not consumed by {TypeMetadataCache<TSaga>.ShortName}");
            }

            return specification.GetMessageSpecification<T>();
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            Observers.All(observer =>
            {
                observer.SagaConfigured(this);
                return true;
            });

            return _messageTypes.Values.SelectMany(x => x.Validate());
        }

        public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TSaga>> specification)
        {
            foreach (ISagaMessageSpecification<TSaga> messageSpecification in _messageTypes.Values)
            {
                messageSpecification.AddPipeSpecification(specification);
            }
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return Observers.Connect(observer);
        }
    }
}
