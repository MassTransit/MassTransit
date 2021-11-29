namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;


    /// <summary>
    /// Copies the input property, as-is, for the property value
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class InputPropertyProvider<TInput, TProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
    {
        readonly IReadProperty<TInput, TProperty> _inputProperty;

        public InputPropertyProvider(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<TProperty>(propertyInfo);
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
