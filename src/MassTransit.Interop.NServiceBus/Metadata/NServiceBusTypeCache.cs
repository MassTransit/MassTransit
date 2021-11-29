namespace MassTransit.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;


    public class NServiceBusTypeCache<T> :
        INServiceBusTypeMetadataCache<T>
    {
        readonly Lazy<string[]> _messageTypeNames;

        NServiceBusTypeCache()
        {
            _messageTypeNames = new Lazy<string[]>(() => GetMessageTypeNames().ToArray());
        }

        public static string[] MessageTypeNames => Cached.Metadata.Value.MessageTypeNames;

        /// <summary>
        /// The names of all the message types supported by the message type
        /// </summary>
        string[] INServiceBusTypeMetadataCache<T>.MessageTypeNames => _messageTypeNames.Value;

        static IEnumerable<string> GetMessageTypeNames()
        {
            foreach (var messageType in MessageTypeCache<T>.MessageTypes)
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
