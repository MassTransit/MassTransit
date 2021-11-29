namespace MassTransit.MessageData.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Initializers;
    using Initializers.PropertyConverters;
    using Initializers.PropertyProviders;


    public class GetMessageDataObjectArrayTransformConfiguration<TInput, TProperty, TElement> :
        IMessageDataTransformConfiguration<TInput>
        where TInput : class
        where TElement : class
    {
        readonly PropertyInfo _property;
        readonly GetMessageDataTransformSpecification<TElement> _transformConfigurator;

        public GetMessageDataObjectArrayTransformConfiguration(IMessageDataRepository repository, IEnumerable<Type> knownTypes, PropertyInfo property)
        {
            _property = property;

            _transformConfigurator = new GetMessageDataTransformSpecification<TElement>(repository, knownTypes);
        }

        public void Apply(ITransformConfigurator<TInput> configurator)
        {
            if (_transformConfigurator.TryGetConverter(out IPropertyConverter<TElement, TElement> converter))
            {
                var inputPropertyProvider = new InputPropertyProvider<TInput, TProperty>(_property);

                IPropertyConverter<TProperty, TProperty> arrayConverter = typeof(TProperty).IsArray
                    ? new ArrayPropertyConverter<TElement, TElement>(converter) as IPropertyConverter<TProperty, TProperty>
                    : new ListPropertyConverter<TElement, TElement>(converter) as IPropertyConverter<TProperty, TProperty>;

                var provider = new PropertyConverterPropertyProvider<TInput, TProperty, TProperty>(arrayConverter, inputPropertyProvider);

                configurator.Set(_property, provider);
            }
        }
    }
}
