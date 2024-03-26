namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            var consumerType = typeof(T);
            if (consumerType.IsGenericType && consumerType.GetGenericTypeDefinition() == typeof(IConsumer<>))
            {
                var messageType = consumerType.GetGenericArguments()[0];
                if (messageType.ClosesType(typeof(Batch<>), out Type[] batchTypes))
                {
                    var interfaceType = new BatchConsumerInterfaceType(messageType, batchTypes[0], consumerType);
                    if (MessageTypeCache.IsValidMessageType(interfaceType.MessageType))
                        yield return interfaceType;
                }
            }

            IEnumerable<IMessageInterfaceType> types = consumerType.GetInterfaces()
                .Where(x => x.IsGenericType)
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
                .Select(x => new BatchConsumerInterfaceType(x.BatchMessageType, x.MessageType, consumerType))
                .Where(x => MessageTypeCache.IsValidMessageType(x.MessageType));

            foreach (var type in types)
                yield return type;
        }
    }
}
