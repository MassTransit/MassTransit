namespace MassTransit.Serialization
{
    using System;
    using System.Linq;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;


    public class EncryptedFallbackMessageDeserializerV2 :
        IMessageDeserializer
    {
        static readonly Type[] ExceptionsToRetry = { typeof(ArgumentOutOfRangeException), typeof(JsonReaderException) };

        readonly IMessageDeserializer _primaryMessageDeserializer;
        readonly IMessageDeserializer _secondaryMessageDeserializer;

        public EncryptedFallbackMessageDeserializerV2(ICryptoStreamProviderV2 primaryCryptoStream, ICryptoStreamProviderV2 secondaryCryptoStream)
        {
            _primaryMessageDeserializer = new EncryptedMessageDeserializerV2(BsonMessageSerializer.Deserializer, primaryCryptoStream);
            _secondaryMessageDeserializer = new EncryptedMessageDeserializerV2(BsonMessageSerializer.Deserializer, secondaryCryptoStream);
        }

        public EncryptedFallbackMessageDeserializerV2(IMessageDeserializer primaryMessageDeserializer, IMessageDeserializer secondaryMessageDeserializer)
        {
            _primaryMessageDeserializer = primaryMessageDeserializer;
            _secondaryMessageDeserializer = secondaryMessageDeserializer;
        }

        public ContentType ContentType => _primaryMessageDeserializer.ContentType;

        public void Probe(ProbeContext context)
        {
            _primaryMessageDeserializer.Probe(context);
            _secondaryMessageDeserializer.Probe(context);
        }

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            return Deserialize(receiveContext, false);
        }

        public SerializerContext Deserialize(MessageBody body, Headers headers, Uri destinationAddress = null)
        {
            return Deserialize(body, headers, false, destinationAddress);
        }

        public MessageBody GetMessageBody(string text)
        {
            return GetMessageBody(text, false);
        }

        ConsumeContext Deserialize(ReceiveContext receiveContext, bool isRetry)
        {
            try
            {
                return _primaryMessageDeserializer.Deserialize(receiveContext);
            }
            catch (SerializationException e) when (ShouldRetry(isRetry, e))
            {
                return Deserialize(receiveContext, true);
            }
            catch
            {
                return _secondaryMessageDeserializer.Deserialize(receiveContext);
            }
        }

        SerializerContext Deserialize(MessageBody body, Headers headers, bool isRetry, Uri destinationAddress = null)
        {
            try
            {
                return _primaryMessageDeserializer.Deserialize(body, headers, destinationAddress);
            }
            catch (SerializationException e) when (ShouldRetry(isRetry, e))
            {
                return Deserialize(body, headers, true, destinationAddress);
            }
            catch
            {
                return _secondaryMessageDeserializer.Deserialize(body, headers, destinationAddress);
            }
        }

        MessageBody GetMessageBody(string text, bool isRetry)
        {
            try
            {
                return _primaryMessageDeserializer.GetMessageBody(text);
            }
            catch (SerializationException e) when (ShouldRetry(isRetry, e))
            {
                return GetMessageBody(text, true);
            }
            catch
            {
                return _secondaryMessageDeserializer.GetMessageBody(text);
            }
        }

        static bool ShouldRetry(bool isRetry, SerializationException e)
        {
            return ExceptionsToRetry.Contains(e.InnerException?.GetType()) && !isRetry;
        }
    }
}
