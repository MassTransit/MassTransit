namespace MassTransit.Courier.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using Newtonsoft.Json.Linq;


    /// <summary>
    /// Serializes an already existing message envelope to the transport, rather than using the message in the SendContext,
    /// setting the headers to the headers in the envelope, and transferring over headers from the context as well.
    /// </summary>
    public class EnvelopeMessageSerializer :
        IMessageSerializer
    {
        static ICryptoStreamProviderV2 _streamProvider;
        static bool _useEncryption;
        readonly MessageEnvelope _envelope;
        readonly object _message;

        public EnvelopeMessageSerializer(ContentType contentType, MessageEnvelope envelope, object message)
        {
            _envelope = envelope;
            _message = message;
            ContentType = _useEncryption ? EncryptedMessageSerializerV2.EncryptedContentType : contentType;
        }

        public ContentType ContentType { get; }

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            try
            {
                var envelope = JObject.FromObject(_envelope, SerializerCache.Serializer);

                if (_message != null)
                {
                    JToken message = JObject.FromObject(_message, SerializerCache.Serializer);
                    envelope["message"] = message.Merge(envelope["message"]);
                }

                context.ContentType = ContentType;

                envelope["sourceAddress"] = context.SourceAddress;

                if (context.MessageId.HasValue)
                    envelope["messageId"] = context.MessageId.Value.ToString();

                if (context.RequestId.HasValue)
                    envelope["requestId"] = context.RequestId.Value.ToString();

                if (context.CorrelationId.HasValue)
                    envelope["correlationId"] = context.CorrelationId.Value.ToString();

                if (context.ConversationId.HasValue)
                    envelope["conversationId"] = context.ConversationId.Value.ToString();

                if (context.InitiatorId.HasValue)
                    envelope["initiatorId"] = context.InitiatorId.Value.ToString();

                if (context.SourceAddress != null)
                    envelope["sourceAddress"] = context.SourceAddress.ToString();

                if (context.DestinationAddress != null)
                    envelope["destinationAddress"] = context.DestinationAddress.ToString();

                if (context.ResponseAddress != null)
                    envelope["responseAddress"] = context.ResponseAddress.ToString();

                if (context.FaultAddress != null)
                    envelope["faultAddress"] = context.FaultAddress.ToString();

                if (context.TimeToLive.HasValue)
                    envelope["expirationTime"] = DateTime.UtcNow + context.TimeToLive;

                if (_useEncryption == false)
                {
                    using var writer = new StreamWriter(stream, JsonMessageSerializer.Encoding, 1024, true);
                    using var jsonWriter = new JsonTextWriter(writer);

                    jsonWriter.Formatting = Formatting.Indented;

                    SerializerCache.Serializer.Serialize(jsonWriter, envelope, typeof(MessageEnvelope));

                    jsonWriter.Flush();
                    writer.Flush();
                }
                else
                {
                    using var cryptoStream = _streamProvider.GetEncryptStream(stream, context);
                    using var jsonWriter = new BsonDataWriter(cryptoStream);

                    BsonMessageSerializer.Serializer.Serialize(jsonWriter, envelope, typeof(MessageEnvelope));

                    jsonWriter.Flush();
                }
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }

        /// <summary>
        /// The purpose of this method is to switch plain json serialization to encrypted serialization in this class.
        /// This is a workaround for the scenario when encryption is enabled globally as without it this class would unconditionally
        /// use plain json serialization not taking the current SendContext serializer configuration into consideration
        /// leaving routing slip event messages unencrypted.
        /// Once the message serializer architecture is evolved to enable layer based approach to add encryption on top of
        /// another serializer, this workaround should be removed.
        /// </summary>
        /// <param name="keyProvider"></param>
        [Obsolete(
            "This method is temporary and is going to be removed in the future once subscription events start using serializer/encryption configured on the endpoint.")]
        public static void UseEncryption(ISecureKeyProvider keyProvider)
        {
            _streamProvider = new AesCryptoStreamProviderV2(keyProvider);
            _useEncryption = true;
        }
    }
}
