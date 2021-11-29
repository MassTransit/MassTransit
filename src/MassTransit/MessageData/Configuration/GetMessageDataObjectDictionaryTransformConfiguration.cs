namespace MassTransit.MessageData.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Initializers;
    using Initializers.PropertyConverters;
    using Initializers.PropertyProviders;


    public class GetMessageDataObjectDictionaryTransformConfiguration<TInput, TProperty, TKey, TValue> :
        IMessageDataTransformConfiguration<TInput>
        where TInput : class
        where TValue : class
    {
        readonly PropertyInfo _property;
        readonly GetMessageDataTransformSpecification<TValue> _transformConfigurator;

        public GetMessageDataObjectDictionaryTransformConfiguration(IMessageDataRepository repository, IEnumerable<Type> knownTypes, PropertyInfo property)
        {
            _property = property;

            _transformConfigurator = new GetMessageDataTransformSpecification<TValue>(repository, knownTypes);
        }

        public void Apply(ITransformConfigurator<TInput> configurator)
        {
            if (_transformConfigurator.TryGetConverter(out IPropertyConverter<TValue, TValue> converter))
            {
                var inputPropertyProvider = new InputPropertyProvider<TInput, TProperty>(_property);

                var dictionaryConverter = new DictionaryPropertyConverter<TKey, TValue, TValue>(converter) as IPropertyConverter<TProperty, TProperty>;

                var provider = new PropertyConverterPropertyProvider<TInput, TProperty, TProperty>(dictionaryConverter, inputPropertyProvider);

                configurator.Set(_property, provider);
            }
        }
    }
}
