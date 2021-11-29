namespace MassTransit.MessageData.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Conventions;
    using Initializers;
    using Internals;
    using MassTransit.Configuration;
    using Middleware;
    using Transformation;


    public class PutMessageDataTransformSpecification<TMessage> :
        TransformSpecification<TMessage>,
        ISendTransformSpecification<TMessage>
        where TMessage : class
    {
        public PutMessageDataTransformSpecification(IMessageDataRepository repository, IEnumerable<Type> knownTypes = null)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            Replace = true;

            var types = new HashSet<Type>(knownTypes ?? Enumerable.Empty<Type>()) { typeof(TMessage) };

            AddMessageDataProperties(repository, types);
        }

        void IPipeSpecification<SendContext<TMessage>>.Apply(IPipeBuilder<SendContext<TMessage>> builder)
        {
            if (Count > 0)
            {
                IMessageInitializer<TMessage> initializer = Build();

                builder.AddFilter(new TransformFilter<TMessage>(initializer));
            }
        }

        public bool TryGetSendTopology(out IMessageSendTopology<TMessage> topology)
        {
            if (Count > 0)
            {
                IMessageInitializer<TMessage> initializer = Build();

                topology = new MessageDataMessageSendTopology<TMessage>(initializer);
                return true;
            }

            topology = default;
            return false;
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

        void AddMessageDataProperties(IMessageDataRepository repository, ICollection<Type> knownTypes)
        {
            foreach (var propertyInfo in MessageTypeCache<TMessage>.Properties)
            {
                var propertyType = propertyInfo.PropertyType;

                void ConfigureDictionary(Type keyType, Type valueType)
                {
                    if (!IsUnknownObjectType(knownTypes, valueType))
                        return;

                    var providerType = typeof(PutMessageDataObjectDictionaryTransformConfiguration<,,,>)
                        .MakeGenericType(typeof(TMessage), propertyType, keyType, valueType);
                    var configuration = (IMessageDataTransformConfiguration<TMessage>)Activator.CreateInstance(providerType, repository, knownTypes,
                        propertyInfo);

                    configuration.Apply(this);
                }

                void ConfigureArray(Type elementType)
                {
                    if (!IsUnknownObjectType(knownTypes, elementType))
                        return;

                    var providerType = typeof(PutMessageDataObjectArrayTransformConfiguration<,,>)
                        .MakeGenericType(typeof(TMessage), propertyType, elementType);
                    var configuration = (IMessageDataTransformConfiguration<TMessage>)Activator.CreateInstance(providerType, repository, knownTypes,
                        propertyInfo);

                    configuration.Apply(this);
                }

                if (propertyType.ClosesType(typeof(MessageData<>), out Type[] types))
                {
                    var providerType = typeof(PutMessageDataTransformConfiguration<,>).MakeGenericType(typeof(TMessage), types[0]);
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
                    ConfigureDictionary(types[0], types[1]);
                else if (propertyType.IsArray)
                    ConfigureArray(propertyType.GetElementType());
                else if (propertyType.ClosesType(typeof(IEnumerable<>), out Type[] enumerableTypes))
                {
                    if (enumerableTypes[0].ClosesType(typeof(KeyValuePair<,>), out types))
                        ConfigureDictionary(types[0], types[1]);
                    else
                        ConfigureArray(enumerableTypes[0]);
                }
                else if (IsUnknownObjectType(knownTypes, propertyType))
                {
                    var providerType = typeof(PutMessageDataObjectTransformConfiguration<,>).MakeGenericType(typeof(TMessage), propertyType);
                    var configuration = (IMessageDataTransformConfiguration<TMessage>)Activator.CreateInstance(providerType, repository, knownTypes,
                        propertyInfo);

                    configuration.Apply(this);
                }
            }
        }

        static bool IsUnknownObjectType(ICollection<Type> knownTypes, Type propertyType)
        {
            return propertyType.IsInterfaceOrConcreteClass() && MessageTypeCache.IsValidMessageType(propertyType) && !propertyType.IsValueTypeOrObject()
                && !knownTypes.Contains(propertyType);
        }
    }
}
