#nullable enable
namespace MassTransit
{
    using System;
    using System.Net.Mime;


    public interface IMessageDeserializer :
        IProbeSite
    {
        ContentType ContentType { get; }

        ConsumeContext Deserialize(ReceiveContext receiveContext);

        SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null);

        /// <summary>
        /// Returns the appropriate message body for the message deserializer, using the input type
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        MessageBody GetMessageBody(string text);
    }
}
