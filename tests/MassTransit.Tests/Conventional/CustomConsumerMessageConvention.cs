namespace MassTransit.Tests.Conventional
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MassTransit.Configuration;


    class CustomConsumerMessageConvention<T> :
        IConsumerMessageConvention
        where T : class
    {
        public IEnumerable<IMessageInterfaceType> GetMessageTypes()
        {
            var consumerType = typeof(T);
            if (consumerType.IsGenericType && consumerType.GetGenericTypeDefinition() == typeof(IHandler<>))
            {
                var interfaceType = new CustomConsumerInterfaceType(consumerType.GetGenericArguments()[0], consumerType);
                if (MessageTypeCache.IsValidMessageType(interfaceType.MessageType))
                    yield return interfaceType;
            }

            IEnumerable<CustomConsumerInterfaceType> types = consumerType.GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IHandler<>))
                .Select(x => new CustomConsumerInterfaceType(x.GetGenericArguments()[0], consumerType))
                .Where(x => MessageTypeCache.IsValidMessageType(x.MessageType));

            foreach (var type in types)
                yield return type;
        }
    }
}
