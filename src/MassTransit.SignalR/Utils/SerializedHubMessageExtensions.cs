namespace MassTransit.SignalR.Utils
{
    using MassTransit.MessageData;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Protocol;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;

    public static class SerializedHubMessageExtensions
    {
        public static SerializedHubMessage ToSerializedHubMessage(this IDictionary<string, byte[]> protocolMessages)
        {
            var count = protocolMessages.Count;
            var serializations = new List<SerializedMessage>(count);

            foreach (var message in protocolMessages)
            {
                var protocol = message.Key;
                var serialized = message.Value;
                serializations.Add(new SerializedMessage(protocol, serialized));
            }

            return new SerializedHubMessage(serializations);
        }

        public static IDictionary<string, byte[]> ToProtocolDictionary(this IReadOnlyList<IHubProtocol> protocols, string methodName, object[] args)
        {
            var serializedMessageHub = new SerializedHubMessage(new InvocationMessage(methodName, args));

            var messages = new Dictionary<string, byte[]>();

            foreach (var protocol in protocols)
            {
                var serialized = serializedMessageHub.GetSerializedMessage(protocol);

                messages.Add(protocol.Name, serialized.ToArray());
            }

            return messages;
        }

        public static async Task<IDictionary<string, byte[]>> ToProtocolDictionary(this MessageData<byte[]> messageData)
        {
            using (var memStream = new MemoryStream(await messageData.Value))
            {
                var binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(memStream) as IDictionary<string, byte[]> ?? throw new ArgumentNullException("Couldn't deserialize MessageData into SignalR Message Dictionary");
            }
        }

        public static async Task<MessageData<byte[]>> ToMessageData(this IDictionary<string, byte[]> protocolMessages, IMessageDataRepository repository)
        {
            using (var memStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memStream, protocolMessages);

                return await repository.PutBytes(memStream.ToArray());
            }
        }
    }
}
