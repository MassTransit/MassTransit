namespace MassTransit.KafkaIntegration.Configuration.Definition
{
    using System;


    public class DefaultTopicNameFormatter :
        ITopicNameFormatter
    {
        protected DefaultTopicNameFormatter()
        {
        }

        public static ITopicNameFormatter Instance { get; } = new DefaultTopicNameFormatter();

        public string GetTopicName<TKey, TValue>()
            where TValue : class
        {
            return GetMessageName(typeof(TValue));
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
