namespace MassTransit.Topology
{
    using System;
    using System.Collections.Concurrent;
    using Configuration;


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
            if (MessageTypeCache<T>.IsValidMessageType == false)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            var specification = _messageTypes.GetOrAdd(typeof(T), CreateMessageTopology<T>);

            return (IMessageTopologyConfigurator<T>)specification;
        }

        protected virtual IMessageTypeTopologyConfigurator CreateMessageTopology<T>(Type type)
            where T : class
        {
            var messageTopology = new MessageTopology<T>(new MessageEntityNameFormatter<T>(EntityNameFormatter));

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

        public MessageTopology(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
        {
            EntityNameFormatter = entityNameFormatter;

            _entityName = new Lazy<string>(() => EntityNameFormatter.FormatEntityName());
        }

        public IMessageEntityNameFormatter<TMessage> EntityNameFormatter { get; private set; }

        public string EntityName => _entityName.Value;

        public void SetEntityNameFormatter(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
        {
            if (entityNameFormatter == null)
                throw new ArgumentNullException(nameof(entityNameFormatter));

            if (_entityName.IsValueCreated)
            {
                if (_entityName.Value == entityNameFormatter.FormatEntityName())
                    return;

                throw new ConfigurationException(
                    $"The message type {TypeCache<TMessage>.ShortName} entity name was already evaluated: {_entityName.Value}");
            }

            EntityNameFormatter = entityNameFormatter;
        }

        public void SetEntityName(string entityName)
        {
            if (entityName == null)
                throw new ArgumentNullException(nameof(entityName));

            SetEntityNameFormatter(new StaticEntityNameFormatter<TMessage>(entityName));
        }
    }
}
