namespace MassTransit.Internals.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Extensions;
    using GraphValidation;
    using Initializers;
    using Initializers.Contexts;
    using Initializers.Factories;
    using Metadata;


    public static class ContractCache
    {
        public static IMessageFactory GetMessageFactory(Contract contract)
        {
            var type = Cached.Builder.GetContractType(contract);

            return CreateMessageFactory(type);
        }

        public static Type GetContractType(Contract contract)
        {
            var type = Cached.Builder.GetContractType(contract);

            return type;
        }

        static IMessageFactory CreateMessageFactory(Type contractType)
        {
            if (!TypeMetadataCache.IsValidMessageType(contractType))
                throw new ArgumentException(nameof(contractType));

            Type[] parameterTypes = new Type[0];
            if (contractType.GetConstructor(parameterTypes) == null)
                throw new ArgumentException("No default constructor available for message type", nameof(contractType));

            return (IMessageFactory)Activator.CreateInstance(typeof(DynamicMessageFactory<>).MakeGenericType(contractType));
        }


        static class Cached
        {
            internal static readonly IContractTypeBuilder Builder = new DynamicContractTypeBuilder();
        }
    }


    public class ContractCache<TMessage> :
        IContractCache<TMessage>
        where TMessage : class
    {
        readonly Contract[] _contracts;
        readonly MessageUrn _contractType;

        ContractCache()
        {
            _contractType = MessageUrn.ForType<TMessage>();
            if (TypeMetadataCache<TMessage>.IsValidMessageType)
                _contracts = GetContracts();
        }

        Contract[] IContractCache<TMessage>.Contracts =>
            _contracts ?? throw new ArgumentException(TypeMetadataCache<TMessage>.InvalidMessageTypeReason, nameof(TMessage));

        public static Contract[] Contracts => Cached.Instance.Value.Contracts;

        public static IMessageFactory<TMessage> Factory => Cached.MessageFactory.Value;

        public static TMessage CreateMessage()
        {
            return Cached.MessageFactory.Value.Create(new BaseInitializeContext(CancellationToken.None)).Message;
        }


        static class Cached
        {
            internal static readonly Lazy<IMessageFactory<TMessage>> MessageFactory = new Lazy<IMessageFactory<TMessage>>(CreateMessageFactory);

            static IMessageFactory<TMessage> CreateMessageFactory()
            {
                if (!TypeMetadataCache<TMessage>.IsValidMessageType)
                    throw new ArgumentException(TypeMetadataCache<TMessage>.InvalidMessageTypeReason, nameof(TMessage));

                var messageType = typeof(TMessage);

                Type[] parameterTypes = new Type[0];
                if (messageType.GetConstructor(parameterTypes) == null)
                    throw new ArgumentException("No default constructor available for message type", nameof(TMessage));

                return (IMessageFactory<TMessage>)Activator.CreateInstance(typeof(DynamicMessageFactory<>).MakeGenericType(messageType));
            }

            internal static readonly Lazy<IContractCache<TMessage>> Instance = new Lazy<IContractCache<TMessage>>(() => new ContractCache<TMessage>());
        }


        Contract[] GetContracts()
        {
            var contractName = _contractType.ToString();

            DependencyGraph<string> graph = new DependencyGraph<string>(1);
            graph.Add(contractName);

            List<Property> propertyList = new List<Property>();

            foreach (var property in TypeMetadataCache<TMessage>.Properties)
            {
                if (property.PropertyType.IsValueTypeOrObject())
                {
                    propertyList.Add(new Property(property.Name, property.PropertyType));
                }
                else if (TypeMetadataCache.IsValidMessageType(property.PropertyType))
                {
                }
            }

            var contract = new Contract(contractName, propertyList.ToArray());

            return new[] {contract};
        }
    }
}
