namespace MassTransit.Interop.NServiceBus.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using MassTransit.Metadata;


    public class NServiceBusTypeCache<T> :
        INServiceBusTypeMetadataCache<T>
    {
        readonly Lazy<string[]> _messageTypeNames;

        NServiceBusTypeCache()
        {
            _messageTypeNames = new Lazy<string[]>(() => GetMessageTypeNames().ToArray());
        }

        /// <summary>
        /// The names of all the message types supported by the message type
        /// </summary>
        string[] INServiceBusTypeMetadataCache<T>.MessageTypeNames => _messageTypeNames.Value;

        public static string[] MessageTypeNames => Cached.Metadata.Value.MessageTypeNames;

        static IEnumerable<string> GetMessageTypeNames()
        {
            foreach (var messageType in TypeMetadataCache<T>.MessageTypes)
            {
                switch (messageType.Name)
                {
                    case "IEvent":
                    case "IMessage":
                    case "ICommand":
                        if (!"NServiceBus".Equals(messageType.Namespace))
                            yield return messageType.AssemblyQualifiedName;
                        break;

                    default:
                        yield return messageType.AssemblyQualifiedName;
                        break;
                }
            }
        }

        static class Cached
        {
            internal static readonly Lazy<INServiceBusTypeMetadataCache<T>> Metadata = new Lazy<INServiceBusTypeMetadataCache<T>>(
                () => new NServiceBusTypeCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}