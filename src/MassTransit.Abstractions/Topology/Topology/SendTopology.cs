namespace MassTransit.Topology
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class SendTopology :
        ISendTopologyConfigurator,
        ISendTopologyConfigurationObserver
    {
        readonly List<IMessageSendTopologyConvention> _conventions;
        readonly object _lock = new object();
        readonly ConcurrentDictionary<Type, Lazy<IMessageSendTopologyConfigurator>> _messageTypes;
        readonly SendTopologyConfigurationObservable _observers;

        public SendTopology()
        {
            _messageTypes = new ConcurrentDictionary<Type, Lazy<IMessageSendTopologyConfigurator>>();

            _observers = new SendTopologyConfigurationObservable();

            _conventions = new List<IMessageSendTopologyConvention>(8);

            DeadLetterQueueNameFormatter = DefaultDeadLetterQueueNameFormatter.Instance;
            ErrorQueueNameFormatter = DefaultErrorQueueNameFormatter.Instance;

            _observers.Connect(this);
        }

        void ISendTopologyConfigurationObserver.MessageTopologyCreated<T>(IMessageSendTopologyConfigurator<T> messageTopology)
        {
            ApplyConventionsToMessageTopology(messageTopology);
        }

        public IDeadLetterQueueNameFormatter DeadLetterQueueNameFormatter { get; set; }
        public IErrorQueueNameFormatter ErrorQueueNameFormatter { get; set; }

        public IMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class
        {
            if (MessageTypeCache<T>.IsValidMessageType == false)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            Lazy<IMessageSendTopologyConfigurator>? specification = _messageTypes.GetOrAdd(typeof(T),
                type => new Lazy<IMessageSendTopologyConfigurator>(() => CreateMessageTopology<T>(type)));

            return (IMessageSendTopologyConfigurator<T>)specification.Value;
        }

        public ConnectHandle ConnectSendTopologyConfigurationObserver(ISendTopologyConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public bool TryAddConvention(ISendTopologyConvention convention)
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

            foreach (Lazy<IMessageSendTopologyConfigurator> messageSendTopologyConfigurator in _messageTypes.Values)
                messageSendTopologyConfigurator.Value.TryAddConvention(convention);

            return true;
        }

        void ISendTopologyConfigurator.AddMessageSendTopology<T>(IMessageSendTopology<T> topology)
        {
            IMessageSendTopologyConfigurator<T> messageConfiguration = GetMessageTopology<T>();

            messageConfiguration.Add(topology);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _messageTypes.Values.SelectMany(x => x.Value.Validate());
        }

        protected virtual IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
            where T : class
        {
            var messageTopology = new MessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);
            return messageTopology;
        }

        protected void OnMessageTopologyCreated<T>(IMessageSendTopologyConfigurator<T> messageTopology)
            where T : class
        {
            _observers.MessageTopologyCreated(messageTopology);
        }

        void ApplyConventionsToMessageTopology<T>(IMessageSendTopologyConfigurator<T> messageTopology)
            where T : class
        {
            IMessageSendTopologyConvention[] conventions;
            lock (_lock)
                conventions = _conventions.ToArray();

            foreach (var convention in conventions)
            {
                if (convention.TryGetMessageSendTopologyConvention(out IMessageSendTopologyConvention<T> messageSendTopologyConvention))
                    messageTopology.TryAddConvention(messageSendTopologyConvention);
            }
        }
    }
}
