namespace MassTransit.ConsumeConnectors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Metadata;


    /// <summary>
    /// A default convention that looks legacy message interface types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LegacyConsumerMessageConvention<T> :
        IConsumerMessageConvention
        where T : class
    {
        public IEnumerable<IMessageInterfaceType> GetMessageTypes()
        {
            if (typeof(T).GetTypeInfo().IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IMessageConsumer<>))
            {
                var interfaceType = new LegacyConsumerInterfaceType(typeof(T).GetGenericArguments()[0], typeof(T));
                if (TypeMetadataCache.IsValidMessageType(interfaceType.MessageType))
                    yield return interfaceType;
            }

            IEnumerable<LegacyConsumerInterfaceType> types = typeof(T).GetInterfaces()
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IMessageConsumer<>))
                .Select(x => new LegacyConsumerInterfaceType(x.GetGenericArguments()[0], typeof(T)))
                .Where(x => TypeMetadataCache.IsValidMessageType(x.MessageType));

            foreach (LegacyConsumerInterfaceType type in types)
                yield return type;
        }
    }
}
