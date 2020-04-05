namespace MassTransit.ConsumeConnectors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Metadata;


    /// <summary>
    /// A default convention that looks for IConsumerOfT message types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncConsumerMessageConvention<T> :
        IConsumerMessageConvention
        where T : class
    {
        public IEnumerable<IMessageInterfaceType> GetMessageTypes()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IConsumer<>))
            {
                var interfaceType = new ConsumerInterfaceType(typeof(T).GetGenericArguments()[0], typeof(T));
                if (TypeMetadataCache.IsValidMessageType(interfaceType.MessageType))
                    yield return interfaceType;
            }

            IEnumerable<IMessageInterfaceType> types = typeof(T).GetInterfaces()
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IConsumer<>))
                .Select(x => new ConsumerInterfaceType(x.GetGenericArguments()[0], typeof(T)))
                .Where(x => TypeMetadataCache.IsValidMessageType(x.MessageType));

            foreach (IMessageInterfaceType type in types)
                yield return type;
        }
    }
}
