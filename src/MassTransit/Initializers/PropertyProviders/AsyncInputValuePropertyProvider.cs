namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Threading.Tasks;
    using Internals.Reflection;


    public class AsyncInputValuePropertyProvider<TInput, TProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
    {
        readonly IReadProperty<TInput, Task<TProperty>> _inputProperty;

        public AsyncInputValuePropertyProvider(string inputPropertyName)
        {
            if (inputPropertyName == null)
                throw new ArgumentNullException(nameof(inputPropertyName));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<Task<TProperty>>(inputPropertyName);
        }

        public async Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            return context.HasInput
                ? await _inputProperty.Get(context.Input).ConfigureAwait(false)
                : default;
        }
    }
}
