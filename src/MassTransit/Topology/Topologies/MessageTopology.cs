namespace MassTransit.Topology.Topologies
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;
    using EntityNameFormatters;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Metadata;
    using Observers;


    public class MessageTopology :
        IMessageTopologyConfigurator
    {
        readonly ConcurrentDictionary<Type, IMessageTypeTopologyConfigurator> _messageTypes;
        readonly MessageTopologyConfigurationObservable _observers;

        public MessageTopology(IEntityNameFormatter entityNameFormatter)
        {
            EntityNameFormatter = entityNameFormatter ?? throw new ArgumentNullException(nameof(entityNameFormatter));

            _messageTypes = new ConcurrentDictionary<Type, IMessageTypeTopologyConfigurator>();
            _observers = new MessageTopologyConfigurationObservable();
        }

        public IEntityNameFormatter EntityNameFormatter { get; private set; }

        public void SetEntityNameFormatter(IEntityNameFormatter entityNameFormatter)
        {
            EntityNameFormatter = entityNameFormatter ?? throw new ArgumentNullException(nameof(entityNameFormatter));
        }

        IMessageTopologyConfigurator<T> IMessageTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        public ConnectHandle ConnectMessageTopologyConfigurationObserver(IMessageTopologyConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        IMessageTopology<T> IMessageTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        IMessageTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class
        {
            if (TypeMetadataCache<T>.IsValidMessageType == false)
                throw new ArgumentException(TypeMetadataCache<T>.InvalidMessageTypeReason, nameof(T));

            var specification = _messageTypes.GetOrAdd(typeof(T), CreateMessageTopology<T>);

            return specification as IMessageTopologyConfigurator<T>;
        }

        protected virtual IMessageTypeTopologyConfigurator CreateMessageTopology<T>(Type type)
            where T : class
        {
            var messageTopology = new MessageTopology<T>(new MessageEntityNameFormatter<T>(EntityNameFormatter), _observers);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }

        void OnMessageTopologyCreated<T>(IMessageTopologyConfigurator<T> messageTopology)
            where T : class
        {
            _observers.MessageTopologyCreated(messageTopology);
        }
    }


    public class MessageTopology<TMessage> :
        IMessageTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly Lazy<string> _entityName;
        readonly MessageTopologyConfigurationObservable _observers;
        readonly ConcurrentDictionary<string, IMessagePropertyTopologyConfigurator<TMessage>> _properties;

        public MessageTopology(IMessageEntityNameFormatter<TMessage> entityNameFormatter, MessageTopologyConfigurationObservable observers)
        {
            _observers = observers;
            EntityNameFormatter = entityNameFormatter;

            _properties = new ConcurrentDictionary<string, IMessagePropertyTopologyConfigurator<TMessage>>(StringComparer.OrdinalIgnoreCase);

            _entityName = new Lazy<string>(() => EntityNameFormatter.FormatEntityName());
        }

        public IMessageEntityNameFormatter<TMessage> EntityNameFormatter { get; private set; }

        public string EntityName => _entityName.Value;

        public void SetEntityNameFormatter(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
        {
            if (_entityName.IsValueCreated)
            {
                if (_entityName.Value == entityNameFormatter.FormatEntityName())
                    return;

                throw new ConfigurationException(
                    $"The message type {TypeMetadataCache<TMessage>.ShortName} entity name was already evaluated: {_entityName.Value}");
            }

            EntityNameFormatter = entityNameFormatter ?? throw new ArgumentNullException(nameof(entityNameFormatter));
        }

        public void SetEntityName(string entityName)
        {
            SetEntityNameFormatter(new StaticEntityNameFormatter<TMessage>(entityName));
        }

        public void CorrelateBy(Expression<Func<TMessage, Guid>> propertyExpression)
        {
            IMessagePropertyTopologyConfigurator<TMessage, Guid> propertyTopology = GetPropertyTopology<Guid>(propertyExpression.GetPropertyInfo());

            propertyTopology.IsCorrelationId = true;
        }

        public IMessagePropertyTopologyConfigurator<TMessage, T> GetProperty<T>(Expression<Func<TMessage, T>> propertyExpression)
        {
            return GetPropertyTopology<T>(propertyExpression.GetPropertyInfo());
        }

        IMessagePropertyTopology<TMessage, T> IMessageTopology<TMessage>.GetProperty<T>(PropertyInfo propertyInfo)
        {
            return GetPropertyTopology<T>(propertyInfo);
        }

        IMessagePropertyTopologyConfigurator<TMessage, T> GetPropertyTopology<T>(PropertyInfo propertyInfo)
        {
            IMessagePropertyTopologyConfigurator<TMessage> specification =
                _properties.GetOrAdd(propertyInfo.Name, _ => CreatePropertyTopology<T>(propertyInfo));

            return specification as IMessagePropertyTopologyConfigurator<TMessage, T>;
        }

        protected virtual IMessagePropertyTopologyConfigurator<TMessage> CreatePropertyTopology<T>(PropertyInfo propertyInfo)
        {
            var messageTopology = new MessagePropertyTopology<TMessage, T>(propertyInfo);

            OnMessagePropertyTopologyCreated(messageTopology);

            return messageTopology;
        }

        void OnMessagePropertyTopologyCreated<T>(IMessagePropertyTopologyConfigurator<TMessage, T> propertyTopology)
        {
            _observers.MessagePropertyTopologyCreated(propertyTopology);
        }
    }
}
