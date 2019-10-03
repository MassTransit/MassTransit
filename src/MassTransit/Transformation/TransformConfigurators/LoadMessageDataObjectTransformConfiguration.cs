namespace MassTransit.Transformation.TransformConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Initializers.PropertyProviders;
    using MessageData;


    public class LoadMessageDataObjectTransformConfiguration<TInput, TProperty> :
        IMessageDataTransformConfiguration<TInput>
        where TInput : class
        where TProperty : class
    {
        readonly PropertyInfo _property;
        readonly MessageDataTransformSpecification<TProperty> _transformConfigurator;

        public LoadMessageDataObjectTransformConfiguration(IMessageDataRepository repository, IEnumerable<Type> knownTypes, PropertyInfo property)
        {
            _property = property;

            _transformConfigurator = new MessageDataTransformSpecification<TProperty>(repository, knownTypes);
        }

        public void Apply(ITransformConfigurator<TInput> configurator)
        {
            if (_transformConfigurator.TryGetConverter(out var converter))
            {
                var inputPropertyProvider = new InputPropertyProvider<TInput, TProperty>(_property);

                var provider = new PropertyConverterPropertyProvider<TInput, TProperty, TProperty>(converter, inputPropertyProvider);

                configurator.Set(_property, provider);
            }
        }
    }
}
