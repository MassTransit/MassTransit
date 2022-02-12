#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Internals;


    public class InstanceConfigurator :
        IInstanceConfigurator,
        IReceiveEndpointSpecification
    {
        readonly object _instance;

        public InstanceConfigurator(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _instance = instance;
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            InstanceConnectorCache.GetInstanceConnector(_instance.GetType()).ConnectInstance(builder, _instance);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (!_instance.GetType().HasInterface<IConsumer>())
                yield return this.Warning($"The instance of {TypeCache.GetShortName(_instance.GetType())} does not implement any consumer interfaces");
        }
    }


    public class InstanceConfigurator<TInstance> :
        IInstanceConfigurator<TInstance>,
        IReceiveEndpointSpecification
        where TInstance : class, IConsumer
    {
        readonly TInstance _instance;
        readonly IConsumerSpecification<TInstance> _specification;

        public InstanceConfigurator(TInstance instance, IConsumerConfigurationObserver observer)
        {
            _instance = instance;

            _specification = ConsumerConnectorCache<TInstance>.Connector.CreateConsumerSpecification<TInstance>();

            _specification.ConnectConsumerConfigurationObserver(observer);
        }

        public void Message<T>(Action<IConsumerMessageConfigurator<T>>? configure)
            where T : class
        {
            IConsumerMessageSpecification<TInstance, T> specification = _specification.GetMessageSpecification<T>();

            configure?.Invoke(specification);
        }

        public void ConsumerMessage<T>(Action<IConsumerMessageConfigurator<TInstance, T>>? configure)
            where T : class
        {
            IConsumerMessageSpecification<TInstance, T> specification = _specification.GetMessageSpecification<T>();

            configure?.Invoke(specification);
        }

        public T Options<T>(Action<T>? configure = null)
            where T : IOptions, new()
        {
            return _specification.Options(configure);
        }

        public T Options<T>(T options, Action<T>? configure = null)
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

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TInstance>> specification)
        {
            _specification.AddPipeSpecification(specification);
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _specification.ConnectConsumerConfigurationObserver(observer);
        }

        public int? ConcurrentMessageLimit
        {
            set => _specification.ConcurrentMessageLimit = value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specification.Validate();
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            InstanceConnectorCache<TInstance>.Connector.ConnectInstance(builder, _instance, _specification);
        }
    }
}
