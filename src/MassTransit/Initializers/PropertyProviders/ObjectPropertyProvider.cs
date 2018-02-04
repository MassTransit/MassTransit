namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Threading.Tasks;
    using Factories;
    using Internals.Reflection;


    public class ObjectPropertyProvider<TInput, TProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
        where TProperty : class
    {
        readonly IReadProperty<TInput, TProperty> _inputProperty;

        public ObjectPropertyProvider(string inputPropertyName)
        {
            if (inputPropertyName == null)
                throw new ArgumentNullException(nameof(inputPropertyName));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<TProperty>(inputPropertyName);
        }

        public async Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            if (!context.HasInput)
                return default;

            var propertyValue = _inputProperty.Get(context.Input);
            if (propertyValue == null)
                return default;

            InitializeContext<TProperty> messageContext = MessageFactoryCache<TProperty>.Factory.Create(context);

            IMessageInitializer<TProperty> initializer = typeof(TInput) == typeof(object)
                ? MessageInitializerCache<TProperty>.GetInitializer(propertyValue.GetType())
                : MessageInitializerCache<TProperty>.GetInitializer(typeof(TInput));

            InitializeContext<TProperty> result = await initializer.Initialize(messageContext, propertyValue).ConfigureAwait(false);

            return result.Message;

        }
    }
}