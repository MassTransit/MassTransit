namespace MassTransit.Configuration
{
    using System;
    using System.Linq;
    using System.Threading;


    public class ConsumerMetadataCache<T> :
        IConsumerMetadataCache<T>
        where T : class
    {
        readonly IMessageInterfaceType[] _consumerTypes;

        ConsumerMetadataCache()
        {
            _consumerTypes = ConsumerConventionCache.GetConventions<T>()
                .SelectMany(x => x.GetMessageTypes())
                .GroupBy(x => x.MessageType)
                .Select(x => x.Last())
                .ToArray();
        }

        public static IMessageInterfaceType[] ConsumerTypes => Cached.Metadata.Value.ConsumerTypes;

        IMessageInterfaceType[] IConsumerMetadataCache<T>.ConsumerTypes => _consumerTypes;


        static class Cached
        {
            internal static readonly Lazy<IConsumerMetadataCache<T>> Metadata = new Lazy<IConsumerMetadataCache<T>>(
                () => new ConsumerMetadataCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
