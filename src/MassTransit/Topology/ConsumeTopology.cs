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
        readonly IList<IMessageConsumeTopologyConvention> _conventions;
        readonly object _lock = new object();
        readonly int _maxQueueNameLength;
        readonly ConcurrentDictionary<Type, IMessageConsumeTopologyConfigurator> _messageTypes;
        readonly ConsumeTopologyConfigurationObservable _observers;

        protected ConsumeTopology(int maxQueueNameLength = 1024)
        {
            _maxQueueNameLength = maxQueueNameLength;
            _messageTypes = new ConcurrentDictionary<Type, IMessageConsumeTopologyConfigurator>();

            _observers = new ConsumeTopologyConfigurationObservable();

            _conventions = new List<IMessageConsumeTopologyConvention>();
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

        public virtual string CreateTemporaryQueueName(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                tag = "endpoint";

            var host = HostMetadataCache.Host;

            var sb = new StringBuilder(host.MachineName.Length + host.ProcessName.Length + tag.Length + 35);

            foreach (var c in host.MachineName)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
            }

            sb.Append('_');
            foreach (var c in host.ProcessName)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
            }

            sb.Append('_');
            sb.Append(tag);
            sb.Append('_');
            sb.Append(NewId.Next().ToString(ZBase32Formatter.LowerCase));

            return ShrinkToFit(sb.ToString(), _maxQueueNameLength);
        }

        IMessageConsumeTopologyConfigurator<T> IConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        public ConnectHandle ConnectConsumeTopologyConfigurationObserver(IConsumeTopologyConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public bool TryAddConvention(IConsumeTopologyConvention convention)
        {
            lock (_lock)
            {
                if (_conventions.Any(x => x.GetType() == convention.GetType()))
                    return false;

                _conventions.Add(convention);

                foreach (var messageConsumeTopologyConfigurator in _messageTypes.Values)
                    messageConsumeTopologyConfigurator.TryAddConvention(convention);

                return true;
            }
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _messageTypes.Values.SelectMany(x => x.Validate());
        }

        void IConsumeTopologyConfigurator.AddMessageConsumeTopology<T>(IMessageConsumeTopology<T> topology)
        {
            IMessageConsumeTopologyConfigurator<T> messageConfiguration = GetMessageTopology<T>();

            messageConfiguration.Add(topology);
        }

        static string ShrinkToFit(string inputName, int maxLength)
        {
            string name;
            if (inputName.Length > maxLength)
            {
                string hashed;
                using (var hasher = new SHA1Managed())
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

            var specification = _messageTypes.GetOrAdd(typeof(T), CreateMessageTopology<T>);

            return specification as IMessageConsumeTopologyConfigurator<T>;
        }

        protected bool All(Func<IMessageConsumeTopologyConfigurator, bool> callback)
        {
            IMessageConsumeTopologyConfigurator[] configurators;
            lock (_lock)
                configurators = _messageTypes.Values.ToArray();

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
                configurators = _messageTypes.Values.ToArray();

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
                configurators = _messageTypes.Values.ToArray();

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

        protected virtual IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
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
    }
}
