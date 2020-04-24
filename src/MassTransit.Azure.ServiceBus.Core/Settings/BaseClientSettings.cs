namespace MassTransit.Azure.ServiceBus.Core.Settings
{
    using System;
    using System.Collections.Generic;
    using Topology;
    using Transport;


    public abstract class BaseClientSettings :
        ClientSettings
    {
        int _maxConcurrentCalls;

        protected BaseClientSettings(IEndpointEntityConfigurator configurator)
        {
            Configurator = configurator;

            PrefetchCount = Defaults.PrefetchCount;
            MaxConcurrentCalls = Defaults.MaxConcurrentCalls;
            MaxAutoRenewDuration = Defaults.MaxAutoRenewDuration;
            MessageWaitTimeout = Defaults.MessageWaitTimeout;
        }

        public IEndpointEntityConfigurator Configurator { get; }

        public bool UsingBasicTier { get; private set; }

        public int MaxConcurrentCalls
        {
            get => _maxConcurrentCalls;
            set
            {
                _maxConcurrentCalls = value;

                if (PrefetchCount > 0 && _maxConcurrentCalls > PrefetchCount)
                    PrefetchCount = _maxConcurrentCalls;
            }
        }

        public int PrefetchCount { get; set; }
        public TimeSpan MaxAutoRenewDuration { get; set; }
        public TimeSpan MessageWaitTimeout { get; set; }

        public abstract TimeSpan LockDuration { get; }
        public abstract bool RequiresSession { get; }
        public abstract string Path { get; }

        public string Name { get; set; }

        public Uri GetInputAddress(Uri serviceUri, string path)
        {
            var builder = new UriBuilder(serviceUri) {Path = path};

            builder.Query += string.Join("&", GetQueryStringOptions());

            return builder.Uri;
        }

        protected abstract IEnumerable<string> GetQueryStringOptions();

        public virtual void SelectBasicTier()
        {
            UsingBasicTier = true;
        }
    }
}
