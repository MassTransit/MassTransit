namespace MassTransit.MessageData.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Initializers;
    using Initializers.PropertyConverters;
    using Initializers.PropertyProviders;


    public class PutMessageDataObjectDictionaryTransformConfiguration<TInput, TProperty, TKey, TValue> :
        IMessageDataTransformConfiguration<TInput>
        where TInput : class
        where TValue : class
    {
        readonly PropertyInfo _property;
        readonly PutMessageDataTransformSpecification<TValue> _transformConfigurator;

        public PutMessageDataObjectDictionaryTransformConfiguration(IMessageDataRepository repository, IEnumerable<Type> knownTypes, PropertyInfo property)
        {
            _property = property;

            _transformConfigurator = new PutMessageDataTransformSpecification<TValue>(repository, knownTypes);
        }

        public void Apply(ITransformConfigurator<TInput> configurator)
        {
            if (_transformConfigurator.TryGetConverter(out IPropertyConverter<TValue, TValue> converter))
            {
                var inputPropertyProvider = new InputPropertyProvider<TInput, TProperty>(_property);

                var dictionaryConverter = new DictionaryPropertyConverter<TKey, TValue, TValue>(converter) as IPropertyConverter<TProperty, TProperty>;

                var provider = new PropertyConverterPropertyProvider<TInput, TProperty, TProperty>(dictionaryConverter, inputPropertyProvider);

                configurator.Transform(_property, provider);
            }
        }
    }
}
