namespace MassTransit.JobService.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using GreenPipes.Internals.Extensions;
    using Metadata;


    public class JobConsumerMessageConvention<T> :
        IConsumerMessageConvention
        where T : class
    {
        public IEnumerable<IMessageInterfaceType> GetMessageTypes()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IJobConsumer<>))
            {
                var interfaceType = new JobInterfaceType(typeof(T).GetGenericArguments()[0], typeof(T));
                if (TypeMetadataCache.IsValidMessageType(interfaceType.MessageType))
                    yield return interfaceType;
            }

            IEnumerable<IMessageInterfaceType> types = typeof(T).GetInterfaces()
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IJobConsumer<>))
                .Select(x => new JobInterfaceType(x.GetGenericArguments()[0], typeof(T)))
                .Where(x => TypeMetadataCache.IsValidMessageType(x.MessageType))
                .Where(x => !x.MessageType.ClosesType(typeof(Batch<>)));

            foreach (var type in types)
                yield return type;
        }
    }
}
