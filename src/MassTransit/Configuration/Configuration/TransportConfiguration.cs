namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


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

        public TransportConfiguration()
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
            if (PrefetchCount < ConcurrentMessageLimit)
                yield return this.Warning("ConcurrentMessageLimit", "Should be <= PrefetchCount");
            if (ConcurrentMessageLimit <= 0)
                yield return this.Failure("ConcurrentMessageLimit", "Must be > 0");
            if (PrefetchCount < 0)
                yield return this.Failure("PrefetchCount", "Must be >= 0");
        }


        class DefaultTransportConfiguration :
            ITransportConfiguration
        {
            public DefaultTransportConfiguration()
            {
                PrefetchCount = Math.Max(Environment.ProcessorCount * 2, 16);
            }

            public ITransportConfigurator Configurator => throw new InvalidOperationException("The default transport configuration cannot be configured");
            public int PrefetchCount { get; }
            public int? ConcurrentMessageLimit => default;

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
