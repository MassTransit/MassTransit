namespace MassTransit.ConsumeConnectors
{
    using System;
    using System.Linq;
    using System.Threading;
    using Util;


    public class ConsumerMetadataCache<T> :
        IConsumerMetadataCache<T>
        where T : class
    {
        readonly IMessageInterfaceType[] _consumerTypes;

        ConsumerMetadataCache()
        {
            _consumerTypes = ConsumerConventionCache.GetConventions<T>()
                .SelectMany(x => x.GetMessageTypes())
                .Distinct((x, y) => x.MessageType == y.MessageType)
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
