namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;


    public class TransportConfiguration :
        ITransportConfiguration,
        ITransportConfigurator
    {
        readonly ITransportConfiguration _parent;
        int? _concurrentMessageLimit;
        int? _prefetchCount;

        public TransportConfiguration(ITransportConfiguration parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            _parent = parent;
        }

        internal TransportConfiguration()
        {
            _parent = new DefaultTransportConfiguration();
        }

        public ITransportConfigurator Configurator => this;

        public int PrefetchCount
        {
            get => _prefetchCount ?? _parent.PrefetchCount;
            set => _prefetchCount = value;
        }

        public int? ConcurrentMessageLimit
        {
            get => _concurrentMessageLimit ?? _parent.ConcurrentMessageLimit;
            set => _concurrentMessageLimit = value;
        }

        public int GetConcurrentMessageLimit()
        {
            return ConcurrentMessageLimit ?? PrefetchCount;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }


        class DefaultTransportConfiguration :
            ITransportConfiguration
        {
            public DefaultTransportConfiguration()
            {
                PrefetchCount = Math.Max(Environment.ProcessorCount * 2, 16);
                ConcurrentMessageLimit = default;
            }

            public ITransportConfigurator Configurator => throw new InvalidOperationException("The default transport configuration cannot be configured");
            public int PrefetchCount { get; }
            public int? ConcurrentMessageLimit { get; }

            public int GetConcurrentMessageLimit()
            {
                return ConcurrentMessageLimit ?? PrefetchCount;
            }

            public IEnumerable<ValidationResult> Validate()
            {
                yield break;
            }
        }
    }
}
