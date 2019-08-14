namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// Copies the input property, as-is, for the property value
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class InputDictionaryPropertyProvider<TInput, TProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class, IDictionary<string, TProperty>
    {
        readonly string _key;

        public InputDictionaryPropertyProvider(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _key = key;
        }

        public Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            if (!context.HasInput)
                return TaskUtil.Default<TProperty>();

            if (context.Input.TryGetValue(_key, out var value))
                return Task.FromResult(value);

            // var matchingKey = context.Input.Keys.FirstOrDefault(x => string.Compare(x, _key, StringComparison.OrdinalIgnoreCase) == 0);
            // if (matchingKey != default && context.Input.TryGetValue(matchingKey, out value))
            //     return Task.FromResult(value);

            return TaskUtil.Default<TProperty>();
        }
    }
}
