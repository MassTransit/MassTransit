namespace MassTransit.SignalR.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Protocol;


    public static class SerializedHubMessageExtensions
    {
        public static SerializedHubMessage ToSerializedHubMessage(this IReadOnlyDictionary<string, byte[]> protocolMessages)
        {
            return new SerializedHubMessage(protocolMessages.Select(message => new SerializedMessage(message.Key, message.Value)).ToList());
        }

        public static IReadOnlyDictionary<string, byte[]> ToProtocolDictionary(this IEnumerable<IHubProtocol> protocols, string methodName, object[] args)
        {
            var serializedMessageHub = new SerializedHubMessage(new InvocationMessage(methodName, args));

            var messages = new Dictionary<string, byte[]>();

            foreach (var protocol in protocols)
            {
                ReadOnlyMemory<byte> serialized = serializedMessageHub.GetSerializedMessage(protocol);

                messages.Add(protocol.Name, serialized.ToArray());
            }

            return messages;
        }
    }
}
