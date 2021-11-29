namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;


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
                if (MessageTypeCache.IsValidMessageType(interfaceType.MessageType))
                    yield return interfaceType;
            }

            IEnumerable<IMessageInterfaceType> types = typeof(T).GetInterfaces()
                .Where(x => IntrospectionExtensions.GetTypeInfo(x).IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IConsumer<>))
                .Select(x => new ConsumerInterfaceType(x.GetGenericArguments()[0], typeof(T)))
                .Where(x => MessageTypeCache.IsValidMessageType(x.MessageType));

            foreach (var type in types)
                yield return type;
        }
    }
}
