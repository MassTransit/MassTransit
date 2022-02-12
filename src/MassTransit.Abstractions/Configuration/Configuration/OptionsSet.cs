namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class OptionsSet :
        IOptionsSet
    {
        readonly IDictionary<Type, IOptions> _options;

        public OptionsSet()
        {
            _options = new Dictionary<Type, IOptions>();
        }

        /// <summary>
        /// Configure the options, adding the option type if it is not present
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The option type</typeparam>
        /// <returns></returns>
        public T Options<T>(Action<T>? configure = null)
            where T : IOptions, new()
        {
            if (_options.TryGetValue(typeof(T), out var existingOptions))
            {
                if (existingOptions is T options)
                {
                    configure?.Invoke(options);
                    return options;
                }

                throw new ArgumentException($"The options type did not match: {TypeCache<T>.ShortName}");
            }
            else
            {
                var options = new T();
                _options.Add(typeof(T), options);

                configure?.Invoke(options);
                return options;
            }
        }

        /// <summary>
        /// Configure the options, adding the option type if it is not present
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The option type</typeparam>
        /// <returns></returns>
        public T Options<T>(T options, Action<T>? configure = null)
            where T : IOptions
        {
            if (_options.TryGetValue(typeof(T), out var existingOptions))
            {
                if (!ReferenceEquals(existingOptions, options))
                    throw new ArgumentException($"The options type was already configured: {TypeCache<T>.ShortName}");
            }
            else
                _options.Add(typeof(T), options);

            configure?.Invoke(options);
            return options;
        }

        /// <summary>
        /// Return the options, if present
        /// </summary>
        /// <param name="options"></param>
        /// <typeparam name="T">The option type</typeparam>
        public bool TryGetOptions<T>(out T options)
            where T : IOptions
        {
            if (_options.TryGetValue(typeof(T), out var existingOptions))
            {
                if (existingOptions is T matchingOptions)
                {
                    options = matchingOptions;
                    return true;
                }

                throw new ArgumentException($"The options type did not match: {TypeCache<T>.ShortName}");
            }

            options = default!;
            return false;
        }

        /// <summary>
        /// Enumerate the options which are assignable to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> SelectOptions<T>()
            where T : class
        {
            foreach (var value in _options.Values)
            {
                if (value is T requested)
                    yield return requested;
            }
        }

        /// <summary>
        /// Enumerate the options which are assignable to the specified type
        /// </summary>
        protected IEnumerable<ValidationResult> ValidateOptions()
        {
            return SelectOptions<ISpecification>().SelectMany(specification => specification.Validate());
        }
    }
}
