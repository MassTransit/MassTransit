namespace MassTransit.MessageData.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Initializers;
    using Initializers.PropertyProviders;


    public class PutMessageDataObjectTransformConfiguration<TInput, TProperty> :
        IMessageDataTransformConfiguration<TInput>
        where TInput : class
        where TProperty : class
    {
        readonly PropertyInfo _property;
        readonly PutMessageDataTransformSpecification<TProperty> _transformConfigurator;

        public PutMessageDataObjectTransformConfiguration(IMessageDataRepository repository, IEnumerable<Type> knownTypes, PropertyInfo property)
        {
            _property = property;

            _transformConfigurator = new PutMessageDataTransformSpecification<TProperty>(repository, knownTypes);
        }

        public void Apply(ITransformConfigurator<TInput> configurator)
        {
            if (_transformConfigurator.TryGetConverter(out IPropertyConverter<TProperty, TProperty> converter))
            {
                var inputPropertyProvider = new InputPropertyProvider<TInput, TProperty>(_property);

                var provider = new PropertyConverterPropertyProvider<TInput, TProperty, TProperty>(converter, inputPropertyProvider);

                configurator.Transform(_property, provider);
            }
        }
    }
}
