namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internals;


    /// <summary>
    /// A convention that looks for IConsumerOfBatchOfT message types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BatchConsumerMessageConvention<T> :
        IConsumerMessageConvention
        where T : class
    {
        public IEnumerable<IMessageInterfaceType> GetMessageTypes()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IConsumer<>))
            {
                var messageType = typeof(T).GetGenericArguments()[0];
                if (messageType.ClosesType(typeof(Batch<>), out Type[] batchTypes))
                {
                    var interfaceType = new BatchConsumerInterfaceType(messageType, batchTypes[0], typeof(T));
                    if (MessageTypeCache.IsValidMessageType(interfaceType.MessageType))
                        yield return interfaceType;
                }
            }

            IEnumerable<IMessageInterfaceType> types = typeof(T).GetInterfaces()
                .Where(x => IntrospectionExtensions.GetTypeInfo(x).IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IConsumer<>))
                .Select(x => new
                {
                    Type = x,
                    MessageType = x.GetGenericArguments()[0]
                })
                .Where(x => x.MessageType.ClosesType(typeof(Batch<>)))
                .Select(x => new
                {
                    x.Type,
                    BatchMessageType = x.MessageType,
                    MessageType = x.MessageType.GetClosingArgument(typeof(Batch<>))
                })
                .Select(x => new BatchConsumerInterfaceType(x.BatchMessageType, x.MessageType, typeof(T)))
                .Where(x => MessageTypeCache.IsValidMessageType(x.MessageType));

            foreach (var type in types)
                yield return type;
        }
    }
}
