namespace MassTransit.Transformation.TransformConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using Initializers;
    using Internals.Extensions;
    using MessageData;
    using Metadata;
    using Pipeline.Filters;
    using PropertyProviders;


    public class MessageDataTransformSpecification<TMessage> :
        TransformSpecification<TMessage>,
        IConsumeTransformSpecification<TMessage>
        where TMessage : class
    {
        public MessageDataTransformSpecification(IMessageDataRepository repository, IEnumerable<Type> knownTypes = null)
        {
            Replace = true;

            var types = new HashSet<Type>(knownTypes ?? Enumerable.Empty<Type>()) {typeof(TMessage)};

            AddMessageDataProperties(repository, types);
        }

        void AddMessageDataProperties(IMessageDataRepository repository, ICollection<Type> knownTypes)
        {
            foreach (var propertyInfo in TypeMetadataCache<TMessage>.Properties)
            {
                var propertyType = propertyInfo.PropertyType;

                void ConfigureDictionary(Type keyType, Type valueType)
                {
                    if (knownTypes.Contains(valueType))
                        return;

                    Type providerType = typeof(LoadMessageDataObjectDictionaryTransformConfiguration<,,,>)
                        .MakeGenericType(typeof(TMessage), propertyType, keyType, valueType);
                    var configuration = (IMessageDataTransformConfiguration<TMessage>)Activator.CreateInstance(providerType, repository, knownTypes,
                        propertyInfo);

                    configuration.Apply(this);
                }

                void ConfigureArray(Type elementType)
                {
                    if (knownTypes.Contains(elementType))
                        return;

                    Type providerType = typeof(LoadMessageDataObjectArrayTransformConfiguration<,,>)
                        .MakeGenericType(typeof(TMessage), propertyType, elementType);
                    var configuration = (IMessageDataTransformConfiguration<TMessage>)Activator.CreateInstance(providerType, repository, knownTypes,
                        propertyInfo);

                    configuration.Apply(this);
                }

                if (propertyType.ClosesType(typeof(MessageData<>), out Type[] types))
                {
                    Type providerType = typeof(LoadMessageDataTransformConfiguration<,>).MakeGenericType(typeof(TMessage), types[0]);
                    var configuration = (IMessageDataTransformConfiguration<TMessage>)Activator.CreateInstance(providerType, repository, propertyInfo);

                    configuration.Apply(this);
                }
                else if (propertyType.IsNullable(out _))
                {
                }
                else if (propertyType.IsValueTypeOrObject())
                {
                }
                else if (propertyType.ClosesType(typeof(IDictionary<,>), out types) || propertyType.ClosesType(typeof(IReadOnlyDictionary<,>), out types))
                {
                    var valueType = types[1];
                    if (valueType.IsInterfaceOrConcreteClass() && TypeMetadataCache.IsValidMessageType(valueType) && !valueType.IsValueTypeOrObject())
                        ConfigureDictionary(types[0], valueType);
                }
                else if (propertyType.IsArray)
                {
                    var elementType = propertyType.GetElementType();
                    if (elementType.IsInterfaceOrConcreteClass() && TypeMetadataCache.IsValidMessageType(elementType) && !elementType.IsValueTypeOrObject())
                        ConfigureArray(elementType);
                }
                else if (propertyType.ClosesType(typeof(IEnumerable<>), out Type[] enumerableTypes))
                {
                    if (enumerableTypes[0].ClosesType(typeof(KeyValuePair<,>), out types))
                    {
                        var valueType = types[1];
                        if (valueType.IsInterfaceOrConcreteClass() && TypeMetadataCache.IsValidMessageType(valueType) && !valueType.IsValueTypeOrObject())
                            ConfigureDictionary(types[0], valueType);
                    }
                    else
                        ConfigureArray(enumerableTypes[0]);
                }
                else if (propertyType.IsInterfaceOrConcreteClass() && TypeMetadataCache.IsValidMessageType(propertyType) && !knownTypes.Contains(propertyType))
                {
                    Type providerType = typeof(LoadMessageDataObjectTransformConfiguration<,>).MakeGenericType(typeof(TMessage), propertyType);
                    var configuration = (IMessageDataTransformConfiguration<TMessage>)Activator.CreateInstance(providerType, repository, knownTypes,
                        propertyInfo);

                    configuration.Apply(this);
                }
            }
        }

        void IPipeSpecification<ConsumeContext<TMessage>>.Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            if (Count > 0)
            {
                var initializer = Build();

                builder.AddFilter(new TransformFilter<TMessage>(initializer));
            }
        }

        public bool TryGetConverter(out IPropertyConverter<TMessage, TMessage> converter)
        {
            if (Count > 0)
            {
                converter = new TransformPropertyConverter<TMessage>(Build());
                return true;
            }

            converter = default;
            return false;
        }
    }
}
