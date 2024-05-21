namespace MassTransit.Topology
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Metadata;


    public class PublishTopology :
        IPublishTopologyConfigurator,
        IPublishTopologyConfigurationObserver
    {
        readonly List<IMessagePublishTopologyConvention> _conventions;
        readonly object _lock = new object();
        readonly ConcurrentDictionary<Type, Lazy<IMessagePublishTopologyConfigurator>> _messageTypes;
        readonly ConcurrentDictionary<Type, IMessageTypeSelector> _messageTypeSelectorCache;
        readonly PublishTopologyConfigurationObservable _observers;

        public PublishTopology()
        {
            _messageTypes = new ConcurrentDictionary<Type, Lazy<IMessagePublishTopologyConfigurator>>();
            _messageTypeSelectorCache = new ConcurrentDictionary<Type, IMessageTypeSelector>();

            _conventions = new List<IMessagePublishTopologyConvention>(8);

            _observers = new PublishTopologyConfigurationObservable();
            _observers.Connect(this);
        }

        void IPublishTopologyConfigurationObserver.MessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> configurator)
        {
            ApplyConventionsToMessageTopology(configurator);
        }

        IMessagePublishTopology<T> IPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        IMessagePublishTopologyConfigurator<T> IPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        public bool TryGetPublishAddress(Type messageType, Uri baseAddress, out Uri? publishAddress)
        {
            return GetMessageTopology(messageType).TryGetPublishAddress(baseAddress, out publishAddress);
        }

        public ConnectHandle ConnectPublishTopologyConfigurationObserver(IPublishTopologyConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public bool TryAddConvention(IPublishTopologyConvention convention)
        {
            var conventionType = convention.GetType();

            lock (_lock)
            {
                for (var i = 0; i < _conventions.Count; i++)
                {
                    if (_conventions[i].GetType() == conventionType)
                        return false;
                }

                _conventions.Add(convention);
            }

            foreach (Lazy<IMessagePublishTopologyConfigurator> messagePublishTopologyConfigurator in _messageTypes.Values)
                messagePublishTopologyConfigurator.Value.TryAddConvention(convention);

            return true;
        }

        void IPublishTopologyConfigurator.AddMessagePublishTopology<T>(IMessagePublishTopology<T> topology)
        {
            IMessagePublishTopologyConfigurator<T> messageConfiguration = GetMessageTopology<T>();

            messageConfiguration.Add(topology);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _messageTypes.Values.SelectMany(x => x.Value.Validate());
        }

        IMessagePublishTopology IPublishTopology.GetMessageTopology(Type messageType)
        {
            return GetMessageTopology(messageType);
        }

        public IMessagePublishTopologyConfigurator GetMessageTopology(Type messageType)
        {
            if (MessageTypeCache.IsValidMessageType(messageType) == false)
                throw new ArgumentException(MessageTypeCache.InvalidMessageTypeReason(messageType), nameof(messageType));

            return _messageTypeSelectorCache.GetOrAdd(messageType, _ => Activation.Activate(messageType, new MessageTypeSelectorFactory(), this))
                .GetMessageTopology();
        }

        protected virtual IMessagePublishTopologyConfigurator CreateMessageTopology<T>()
            where T : class
        {
            var messageTopology = new MessagePublishTopology<T>(this);

            var connector = new ImplementedMessageTypeConnector(this);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }

        protected IMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class
        {
            if (MessageTypeCache<T>.IsValidMessageType == false)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            Lazy<IMessagePublishTopologyConfigurator> topology =
                _messageTypes.GetOrAdd(typeof(T), _ => new Lazy<IMessagePublishTopologyConfigurator>(() => CreateMessageTopology<T>()));

            return (IMessagePublishTopologyConfigurator<T>)topology.Value;
        }

        protected void OnMessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> messageTopology)
            where T : class
        {
            _observers.MessageTopologyCreated(messageTopology);
        }

        protected void ForEachMessageType<T>(Action<T> callback)
        {
            foreach (Lazy<IMessagePublishTopologyConfigurator> configurator in _messageTypes.Values)
                callback((T)configurator.Value);
        }

        void ApplyConventionsToMessageTopology<T>(IMessagePublishTopologyConfigurator<T> messageTopology)
            where T : class
        {
            IMessagePublishTopologyConvention[] conventions;
            lock (_lock)
                conventions = _conventions.ToArray();

            foreach (var convention in conventions)
            {
                if (convention.TryGetMessagePublishTopologyConvention(out IMessagePublishTopologyConvention<T> messagePublishTopologyConvention))
                    messageTopology.TryAddConvention(messagePublishTopologyConvention);
            }
        }


        class ImplementedMessageTypeConnector :
            IImplementedMessageType
        {
            readonly IPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(IPublishTopologyConfigurator publishTopology)
            {
                _publishTopology = publishTopology;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                _publishTopology.GetMessageTopology<T>();
            }
        }


        readonly struct MessageTypeSelectorFactory :
            IActivationType<IMessageTypeSelector, PublishTopology>
        {
            public IMessageTypeSelector ActivateType<T>(PublishTopology consumeTopology)
                where T : class
            {
                return new MessageTypeSelector<T>(consumeTopology);
            }
        }


        interface IMessageTypeSelector
        {
            IMessagePublishTopologyConfigurator GetMessageTopology();
        }


        class MessageTypeSelector<T> :
            IMessageTypeSelector
            where T : class
        {
            readonly PublishTopology _publishTopology;

            public MessageTypeSelector(PublishTopology publishTopology)
            {
                _publishTopology = publishTopology;
            }

            public IMessagePublishTopologyConfigurator GetMessageTopology()
            {
                return _publishTopology.GetMessageTopology<T>();
            }
        }
    }
}
