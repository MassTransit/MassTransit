namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    /// <summary>
    /// Copies the input property, as-is, for the property value
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class InputDictionaryValuePropertyProvider<TInput, TProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class, IDictionary<string, TProperty>
    {
        readonly string _key;

        public InputDictionaryValuePropertyProvider(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _key = key;
        }

        public Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            return Task.FromResult(context.HasInput && context.Input.TryGetValue(_key, out var value)
                ? value
                : default);
        }
    }
}
