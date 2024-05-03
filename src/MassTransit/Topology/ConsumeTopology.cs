namespace MassTransit
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Configuration;
    using Metadata;
    using NewIdFormatters;


    public class ConsumeTopology :
        IConsumeTopologyConfigurator,
        IConsumeTopologyConfigurationObserver
    {
        readonly List<IMessageConsumeTopologyConvention> _conventions;
        readonly object _lock = new();
        readonly int _maxQueueNameLength;
        readonly ConcurrentDictionary<Type, Lazy<IMessageConsumeTopologyConfigurator>> _messageTypes;
        readonly ConcurrentDictionary<Type, IMessageTypeSelector> _messageTypeSelectorCache;
        readonly ConsumeTopologyConfigurationObservable _observers;

        protected ConsumeTopology(int maxQueueNameLength = 1024)
        {
            _maxQueueNameLength = maxQueueNameLength;

            _messageTypes = new ConcurrentDictionary<Type, Lazy<IMessageConsumeTopologyConfigurator>>();
            _messageTypeSelectorCache = new ConcurrentDictionary<Type, IMessageTypeSelector>();

            _conventions = new List<IMessageConsumeTopologyConvention>(8);

            _observers = new ConsumeTopologyConfigurationObservable();
            _observers.Connect(this);
        }

        void IConsumeTopologyConfigurationObserver.MessageTopologyCreated<T>(IMessageConsumeTopologyConfigurator<T> messageTopology)
        {
            ApplyConventionsToMessageTopology(messageTopology);
        }

        IMessageConsumeTopology<T> IConsumeTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        IMessageConsumeTopologyConfigurator<T> IConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        public IMessageConsumeTopologyConfigurator GetMessageTopology(Type messageType)
        {
            return _messageTypeSelectorCache.GetOrAdd(messageType, _ => Activation.Activate(messageType, new MessageTypeSelectorFactory(), this))
                .GetMessageTopology();
        }

        public virtual string CreateTemporaryQueueName(string tag)
        {
            return ShrinkToFit(DefaultEndpointNameFormatter.GetTemporaryQueueName(tag), _maxQueueNameLength);
        }

        public ConnectHandle ConnectConsumeTopologyConfigurationObserver(IConsumeTopologyConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public bool TryAddConvention(IConsumeTopologyConvention convention)
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

            foreach (Lazy<IMessageConsumeTopologyConfigurator> messageConsumeTopologyConfigurator in _messageTypes.Values)
                messageConsumeTopologyConfigurator.Value.TryAddConvention(convention);

            return true;
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _messageTypes.Values.SelectMany(x => x.Value.Validate());
        }

        static string ShrinkToFit(string inputName, int maxLength)
        {
            string name;
            if (inputName.Length > maxLength)
            {
                string hashed;
                using (var hasher = SHA1.Create())
                {
                    var buffer = Encoding.UTF8.GetBytes(inputName);
                    var hash = hasher.ComputeHash(buffer);
                    hashed = ZBase32Formatter.LowerCase.Format(hash).Substring(0, 6);
                }

                name = $"{inputName.Substring(0, maxLength - 7)}-{hashed}";
            }
            else
                name = inputName;

            return name;
        }

        protected IMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class
        {
            if (MessageTypeCache<T>.IsValidMessageType == false)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            Lazy<IMessageConsumeTopologyConfigurator> specification = _messageTypes.GetOrAdd(typeof(T),
                _ => new Lazy<IMessageConsumeTopologyConfigurator>(() => CreateMessageTopology<T>()));

            return specification.Value as IMessageConsumeTopologyConfigurator<T>;
        }

        protected bool All(Func<IMessageConsumeTopologyConfigurator, bool> callback)
        {
            IMessageConsumeTopologyConfigurator[] configurators;
            lock (_lock)
                configurators = _messageTypes.Values.Select(x => x.Value).ToArray();

            if (configurators.Length == 0)
                return true;

            if (configurators.Length == 1)
                return callback(configurators[0]);

            return configurators.All(callback);
        }

        protected IEnumerable<TResult> SelectMany<T, TResult>(Func<T, IEnumerable<TResult>> selector)
            where T : class
        {
            IMessageConsumeTopologyConfigurator[] configurators;
            lock (_lock)
                configurators = _messageTypes.Values.Select(x => x.Value).ToArray();

            if (configurators.Length == 0)
                return Enumerable.Empty<TResult>();

            if (configurators.Length == 1)
                return selector(configurators[0] as T);

            return configurators.Cast<T>().SelectMany(selector);
        }

        protected void ForEach<T>(Action<T> callback)
            where T : class
        {
            IMessageConsumeTopologyConfigurator[] configurators;
            lock (_lock)
                configurators = _messageTypes.Values.Select(x => x.Value).ToArray();

            switch (configurators.Length)
            {
                case 0:
                    break;
                case 1:
                    callback(configurators[0] as T);
                    break;
                default:
                    foreach (var configurator in configurators.Cast<T>())
                        callback(configurator);

                    break;
            }
        }

        protected virtual IMessageConsumeTopologyConfigurator CreateMessageTopology<T>()
            where T : class
        {
            var messageTopology = new MessageConsumeTopology<T>();

            OnMessageTopologyCreated(messageTopology);
            return messageTopology;
        }

        protected void OnMessageTopologyCreated<T>(IMessageConsumeTopologyConfigurator<T> messageTopology)
            where T : class
        {
            _observers.MessageTopologyCreated(messageTopology);
        }

        void ApplyConventionsToMessageTopology<T>(IMessageConsumeTopologyConfigurator<T> messageTopology)
            where T : class
        {
            IMessageConsumeTopologyConvention[] conventions;
            lock (_lock)
                conventions = _conventions.ToArray();

            foreach (var convention in conventions)
            {
                if (convention.TryGetMessageConsumeTopologyConvention(out IMessageConsumeTopologyConvention<T> messageConsumeTopologyConvention))
                    messageTopology.TryAddConvention(messageConsumeTopologyConvention);
            }
        }


        readonly struct MessageTypeSelectorFactory :
            IActivationType<IMessageTypeSelector, ConsumeTopology>
        {
            public IMessageTypeSelector ActivateType<T>(ConsumeTopology consumeTopology)
                where T : class
            {
                return new MessageTypeSelector<T>(consumeTopology);
            }
        }


        interface IMessageTypeSelector
        {
            IMessageConsumeTopologyConfigurator GetMessageTopology();
        }


        class MessageTypeSelector<T> :
            IMessageTypeSelector
            where T : class
        {
            readonly ConsumeTopology _consumeTopology;

            public MessageTypeSelector(ConsumeTopology consumeTopology)
            {
                _consumeTopology = consumeTopology;
            }

            public IMessageConsumeTopologyConfigurator GetMessageTopology()
            {
                return _consumeTopology.GetMessageTopology<T>();
            }
        }
    }
}
