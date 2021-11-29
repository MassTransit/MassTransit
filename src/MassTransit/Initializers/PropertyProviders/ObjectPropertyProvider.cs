namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Util;


    public class ObjectPropertyProvider<TInput, TProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
        where TProperty : class
    {
        readonly ConcurrentDictionary<Type, Converter> _converters;
        readonly IPropertyProviderFactory<TInput> _factory;
        readonly IPropertyProvider<TInput, object> _provider;

        public ObjectPropertyProvider(IPropertyProviderFactory<TInput> factory, IPropertyProvider<TInput, object> provider)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));

            _converters = new ConcurrentDictionary<Type, Converter>();
        }

        public Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            Task<object> propertyTask = _provider.GetProperty(context);
            if (propertyTask.Status == TaskStatus.RanToCompletion)
            {
                var propertyValue = propertyTask.Result;
                if (propertyValue == default)
                    return TaskUtil.Default<TProperty>();

                var converter = _converters.GetOrAdd(propertyValue.GetType(), CreateConverter);

                return converter.Convert(context, propertyValue);
            }

            async Task<TProperty> GetPropertyAsync()
            {
                var propertyValue = await propertyTask.ConfigureAwait(false);

                var converter = _converters.GetOrAdd(propertyValue.GetType(), CreateConverter);

                return await converter.Convert(context, propertyValue).ConfigureAwait(false);
            }

            return GetPropertyAsync();
        }

        Converter CreateConverter(Type type)
        {
            return (Converter)Activator.CreateInstance(typeof(ObjectConverter<>).MakeGenericType(typeof(TInput), typeof(TProperty), type), _factory);
        }


        interface Converter
        {
            Task<TProperty> Convert<T>(InitializeContext<T, TInput> context, object propertyValue)
                where T : class;
        }


        class ObjectConverter<TObject> :
            Converter
        {
            readonly IPropertyConverter<TProperty, TObject> _converter;

            public ObjectConverter(IPropertyProviderFactory<TInput> factory)
            {
                factory.TryGetPropertyConverter(out _converter);
            }

            public Task<TProperty> Convert<T>(InitializeContext<T, TInput> context, object propertyValue)
                where T : class
            {
                return _converter == null
                    ? TaskUtil.Default<TProperty>()
                    : _converter.Convert(context, (TObject)propertyValue);
            }
        }
    }
}
