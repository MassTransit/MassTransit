namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using System.Linq;


    public class CombinedEndpointDefinition :
        IEndpointDefinition
    {
        readonly IReadOnlyList<IEndpointDefinition> _definitions;

        internal CombinedEndpointDefinition(IReadOnlyList<IEndpointDefinition> definitions)
        {
            _definitions = definitions;

            if (_definitions.All(x => x.ConfigureConsumeTopology))
                ConfigureConsumeTopology = true;
            else if (_definitions.All(x => x.ConfigureConsumeTopology == false))
                ConfigureConsumeTopology = false;
            else
            {
                throw new ConfigurationException(
                    $"Endpoints are not aligned on ConfigureConsumeTopology: {string.Join(", ", _definitions.Select(x => TypeCache.GetShortName(x.GetType())))}");
            }
        }

        public bool IsTemporary => _definitions.All(x => x.IsTemporary);

        public int? PrefetchCount
        {
            get
            {
                int? prefetch = default;
                foreach (var definition in _definitions)
                {
                    if (definition.PrefetchCount.HasValue)
                    {
                        if (prefetch == null)
                            prefetch = definition.PrefetchCount;
                        else if (definition.PrefetchCount.Value > prefetch)
                            prefetch = definition.PrefetchCount.Value;
                    }
                }

                return prefetch;
            }
        }

        public int? ConcurrentMessageLimit
        {
            get
            {
                int? concurrentMessageLimit = default;
                foreach (var definition in _definitions)
                {
                    if (definition.ConcurrentMessageLimit.HasValue)
                    {
                        if (concurrentMessageLimit == null)
                            concurrentMessageLimit = definition.ConcurrentMessageLimit;
                        else if (definition.ConcurrentMessageLimit.Value > concurrentMessageLimit)
                            concurrentMessageLimit = definition.ConcurrentMessageLimit.Value;
                    }
                }

                return concurrentMessageLimit;
            }
        }

        public bool ConfigureConsumeTopology { get; }

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return _definitions.FirstOrDefault()?.GetEndpointName(formatter);
        }

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            foreach (var definition in _definitions)
                definition.Configure(configurator);
        }
    }
}
