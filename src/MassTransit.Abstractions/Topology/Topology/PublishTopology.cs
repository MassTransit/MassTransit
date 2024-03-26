using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MassTransit.Configuration;
using MassTransit.Metadata;

namespace MassTransit.Topology
{
    public class PublishTopology :
        IPublishTopologyConfigurator,
        IPublishTopologyConfigurationObserver
    {
        readonly List<IMessagePublishTopologyConvention> _conventions;
        readonly object _lock = new object();
        readonly ConcurrentDictionary<Type, IMessageTypeFactory> _messageTypeFactoryCache;
        readonly ConcurrentDictionary<Type, Lazy<IMessagePublishTopologyConfigurator>> _messageTypes;
        readonly PublishTopologyConfigurationObservable _observers;

        public PublishTopology()
        {
            _messageTypes = new ConcurrentDictionary<Type, Lazy<IMessagePublishTopologyConfigurator>>();

            _observers = new PublishTopologyConfigurationObservable();
            _messageTypeFactoryCache = new ConcurrentDictionary<Type, IMessageTypeFactory>();

            _conventions = new List<IMessagePublishTopologyConvention>(8);

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
            return _messageTypes.GetOrAdd(messageType, type => new Lazy<IMessagePublishTopologyConfigurator>(() => CreateMessageType(type)))
                .Value.TryGetPublishAddress(baseAddress, out publishAddress);
        }

        public ConnectHandle ConnectPublishTopologyConfigurationObserver(IPublishTopologyConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public bool TryAddConvention(IPublishTopologyConvention convention)
        {
            lock (_lock)
            {
                var conventionType = convention.GetType();

                for (var i = 0; i < _conventions.Count; i++)
                {
                    if (_conventions[i].GetType() == conventionType)
                        return false;
                }

                _conventions.Add(convention);
                return true;
            }
        }

        void IPublishTopologyConfigurator.AddMessagePublishTopology<T>(IMessagePublishTopology<T> topology)
        {
            var messageConfiguration = GetMessageTopology<T>();

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

            return GetOrAddByMessageType(messageType).CreateMessageType();
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

            var topology = _messageTypes.GetOrAdd(typeof(T), type => new Lazy<IMessagePublishTopologyConfigurator>(() => CreateMessageTopology<T>()));

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
            {
                conventions = _conventions.ToArray();
            }

            foreach (var convention in conventions)
                if (convention.TryGetMessagePublishTopologyConvention(out IMessagePublishTopologyConvention<T> messagePublishTopologyConvention))
                    messageTopology.TryAddConvention(messagePublishTopologyConvention);
        }

        IMessagePublishTopologyConfigurator CreateMessageType(Type messageType)
        {
            return GetOrAddByMessageType(messageType).CreateMessageType();
        }

        IMessageTypeFactory GetOrAddByMessageType(Type type)
        {
            return _messageTypeFactoryCache.GetOrAdd(type, _ => Activation.Activate(type, new Factory(), this));
        }


        readonly struct Factory :
            IActivationType<IMessageTypeFactory, IPublishTopologyConfigurator>
        {
            public IMessageTypeFactory ActivateType<T>(IPublishTopologyConfigurator configurator) where T : class
            {
                return new MessageTypeFactory<T>(configurator);
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


        interface IMessageTypeFactory
        {
            IMessagePublishTopologyConfigurator CreateMessageType();
        }


        class MessageTypeFactory<T> :
            IMessageTypeFactory
            where T : class
        {
            readonly IPublishTopologyConfigurator _configurator;

            public MessageTypeFactory(IPublishTopologyConfigurator configurator)
            {
                _configurator = configurator;
            }

            public IMessagePublishTopologyConfigurator CreateMessageType()
            {
                return _configurator.GetMessageTopology<T>();
            }
        }
    }
}
