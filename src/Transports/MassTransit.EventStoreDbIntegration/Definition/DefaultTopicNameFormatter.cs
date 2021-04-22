using System;
using MassTransit.Topology;

namespace MassTransit.EventStoreDbIntegration.Definition
{
    public class DefaultTopicNameFormatter :
        IEntityNameFormatter
    {
        protected DefaultTopicNameFormatter()
        {
        }

        public static IEntityNameFormatter Instance { get; } = new DefaultTopicNameFormatter();

        public string FormatEntityName<T>()
        {
            return GetMessageName(typeof(T));
        }

        protected virtual string SanitizeName(string name)
        {
            return name.ToLowerInvariant();
        }

        string GetMessageName(Type type)
        {
            if (type.IsGenericType)
                return SanitizeName(type.GetGenericArguments()[0].Name);

            var messageName = type.Name;

            return SanitizeName(messageName);
        }
    }
}
