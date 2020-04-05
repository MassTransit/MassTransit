namespace MassTransit.ConsumeConfigurators
{
    using System;
    using System.Collections.Generic;
    using Metadata;


    public class OptionsSet
    {
        readonly IDictionary<Type, IOptions> _options;

        public OptionsSet()
        {
            _options = new Dictionary<Type, IOptions>();
        }

        public T Configure<T>(Action<T> configure = null)
            where T : IOptions, new()
        {
            if (_options.TryGetValue(typeof(T), out var existingOptions))
            {
                if (existingOptions is T options)
                {
                    configure?.Invoke(options);
                    return options;
                }

                throw new ArgumentException($"The options type did not match: {TypeMetadataCache<T>.ShortName}");
            }
            else
            {
                var options = new T();
                _options.Add(typeof(T), options);

                configure?.Invoke(options);
                return options;
            }
        }
    }
}
