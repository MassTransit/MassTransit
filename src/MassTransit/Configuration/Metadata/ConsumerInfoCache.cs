namespace MassTransit.Metadata
{
    using System;
    using System.Linq;
    using ConsumeConnectors;
    using Contracts;


    public class ConsumerInfoCache<TConsumer> :
        IConsumerInfoCache<TConsumer>
        where TConsumer : class
    {
        readonly Lazy<ConsumerInfo> _consumerInfo;

        public ConsumerInfoCache()
        {
            _consumerInfo = new Lazy<ConsumerInfo>(CreateConsumerInfo);
        }

        public static ConsumerInfo ConsumerInfo => Cached.Instance.Value.ConsumerInfo;

        static ConsumerInfo CreateConsumerInfo()
        {
            var consumerType = TypeMetadataCache<TConsumer>.ShortName;
            MessageInfo[] messages = ConsumerMetadataCache<TConsumer>.ConsumerTypes.Select(x => MessageInfoCache.GetMessageInfo(x.MessageType)).ToArray();

            return new CachedConsumerInfo(consumerType, messages);
        }


        static class Cached
        {
            internal static readonly Lazy<IConsumerInfoCache<TConsumer>> Instance =
                new Lazy<IConsumerInfoCache<TConsumer>>(() => new ConsumerInfoCache<TConsumer>());
        }


        class CachedConsumerInfo :
            ConsumerInfo
        {
            public CachedConsumerInfo(string consumerType, MessageInfo[] messages)
            {
                ConsumerType = consumerType;
                Messages = messages;
            }

            public string ConsumerType { get; }
            public MessageInfo[] Messages { get; }
        }


        ConsumerInfo IConsumerInfoCache.ConsumerInfo => _consumerInfo.Value;
    }
}
