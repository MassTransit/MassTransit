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
        readonly IList<IMessageSendTopologyConvention> _conventions;
        readonly object _lock = new object();
        readonly ConcurrentDictionary<Type, IMessageSendTopologyConfigurator> _messageTypes;
        readonly SendTopologyConfigurationObservable _observers;

        public SendTopology()
        {
            _messageTypes = new ConcurrentDictionary<Type, IMessageSendTopologyConfigurator>();

            _observers = new SendTopologyConfigurationObservable();

            _conventions = new List<IMessageSendTopologyConvention>();
            _observers.Connect(this);
        }

        void ISendTopologyConfigurationObserver.MessageTopologyCreated<T>(IMessageSendTopologyConfigurator<T> messageTopology)
        {
            ApplyConventionsToMessageTopology(messageTopology);
        }

        public IMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class
        {
            if (MessageTypeCache<T>.IsValidMessageType == false)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            var specification = _messageTypes.GetOrAdd(typeof(T), CreateMessageTopology<T>);

            return (IMessageSendTopologyConfigurator<T>)specification;
        }

        public ConnectHandle ConnectSendTopologyConfigurationObserver(ISendTopologyConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public bool TryAddConvention(ISendTopologyConvention convention)
        {
            lock (_lock)
            {
                if (_conventions.Any(x => x.GetType() == convention.GetType()))
                    return false;
                _conventions.Add(convention);
                return true;
            }
        }

        void ISendTopologyConfigurator.AddMessageSendTopology<T>(IMessageSendTopology<T> topology)
        {
            IMessageSendTopologyConfigurator<T> messageConfiguration = GetMessageTopology<T>();

            messageConfiguration.Add(topology);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _messageTypes.Values.SelectMany(x => x.Validate());
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
