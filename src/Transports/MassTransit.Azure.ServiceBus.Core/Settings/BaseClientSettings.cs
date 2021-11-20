namespace MassTransit.Azure.ServiceBus.Core.Settings
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Topology;
    using Transport;


    public abstract class BaseClientSettings :
        ClientSettings
    {
        readonly IServiceBusEndpointConfiguration _configuration;

        protected BaseClientSettings(IServiceBusEndpointConfiguration configuration, IEndpointEntityConfigurator configurator)
        {
            _configuration = configuration;
            Configurator = configurator;

            MaxAutoRenewDuration = Defaults.MaxAutoRenewDuration;
            SessionIdleTimeout = Defaults.SessionIdleTimeout;
        }

        public IEndpointEntityConfigurator Configurator { get; }

        public bool UsingBasicTier { get; private set; }

        public abstract bool RequiresSession { get; }

        public TimeSpan SessionIdleTimeout { get; set; }

        public int MaxConcurrentCalls => Math.Max(_configuration.Transport.GetConcurrentMessageLimit(), 1);
        public int PrefetchCount => _configuration.Transport.PrefetchCount;

        public TimeSpan MaxAutoRenewDuration { get; set; }

        public abstract string Path { get; }

        public string Name { get; set; }

        public Uri GetInputAddress(Uri serviceUri, string path)
        {
            var builder = new UriBuilder(serviceUri) { Path = path };

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
