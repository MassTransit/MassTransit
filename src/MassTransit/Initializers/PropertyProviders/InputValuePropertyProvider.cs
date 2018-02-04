namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Threading.Tasks;
    using Internals.Reflection;


    /// <summary>
    /// Copies the input property, as-is, for the property value
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class InputValuePropertyProvider<TInput, TProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
    {
        readonly IReadProperty<TInput, TProperty> _inputProperty;

        public InputValuePropertyProvider(string inputPropertyName)
        {
            if (inputPropertyName == null)
                throw new ArgumentNullException(nameof(inputPropertyName));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<TProperty>(inputPropertyName);
        }

        public Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            return Task.FromResult(context.HasInput
                ? _inputProperty.Get(context.Input)
                : default);
        }
    }
}
