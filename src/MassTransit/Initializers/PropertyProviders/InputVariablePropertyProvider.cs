namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Threading.Tasks;
    using Internals.Reflection;
    using Variables;


    /// <summary>
    /// Copies the input property, as-is, for the property value
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class InputVariablePropertyProvider<TInput, TProperty, TValue> :
        IPropertyProvider<TInput, TValue>
        where TInput : class
        where TProperty : IInitializerVariable<TValue>
    {
        readonly IReadProperty<TInput, TProperty> _inputProperty;

        public InputVariablePropertyProvider(string inputPropertyName)
        {
            if (inputPropertyName == null)
                throw new ArgumentNullException(nameof(inputPropertyName));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<TProperty>(inputPropertyName);
        }

        public async Task<TValue> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            if (!context.HasInput)
                return default;

            var property = _inputProperty.Get(context.Input);
            if (property == null)
                return default;

            var propertyValue = await property.GetValue(context).ConfigureAwait(false);

            return propertyValue;
        }
    }
}