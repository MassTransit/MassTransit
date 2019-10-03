namespace MassTransit.Transformation.TransformConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Initializers;
    using Initializers.PropertyConverters;
    using Initializers.PropertyProviders;
    using MessageData;


    public class LoadMessageDataObjectDictionaryTransformConfiguration<TInput, TProperty, TKey, TValue> :
        IMessageDataTransformConfiguration<TInput>
        where TInput : class
        where TValue : class
    {
        readonly PropertyInfo _property;
        readonly MessageDataTransformSpecification<TValue> _transformConfigurator;

        public LoadMessageDataObjectDictionaryTransformConfiguration(IMessageDataRepository repository, IEnumerable<Type> knownTypes, PropertyInfo property)
        {
            _property = property;

            _transformConfigurator = new MessageDataTransformSpecification<TValue>(repository, knownTypes);
        }

        public void Apply(ITransformConfigurator<TInput> configurator)
        {
            if (_transformConfigurator.TryGetConverter(out var converter))
            {
                var inputPropertyProvider = new InputPropertyProvider<TInput, TProperty>(_property);

                var dictionaryConverter = new DictionaryPropertyConverter<TKey, TValue, TValue>(converter) as IPropertyConverter<TProperty, TProperty>;

                var provider = new PropertyConverterPropertyProvider<TInput, TProperty, TProperty>(dictionaryConverter, inputPropertyProvider);

                configurator.Set(_property, provider);
            }
        }
    }
}
