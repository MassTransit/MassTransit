#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using ProtoBuf.Meta;

    public class ProtobufMessageDeserializer<TProtoMessage> : IMessageDeserializer
        where TProtoMessage : class
    {
        private readonly RuntimeTypeModel _typeModel;
        private readonly IObjectDeserializer _objectDeserializer;

        public ProtobufMessageDeserializer(RuntimeTypeModel typeModel)
        {
            _typeModel = typeModel;
            _objectDeserializer = new ProtobufObjectDeserializer(typeModel);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("protobuf");
            scope.Add("contentType", ContentType.MediaType);
        }

        public ContentType ContentType => new ContentType("application/vnd.masstransit+pbuf");

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            return new BodyConsumeContext(receiveContext, Deserialize(receiveContext.Body, receiveContext.TransportHeaders, receiveContext.InputAddress));
        }

        public SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null)
        {
            try
            {
                using var stream = body.GetStream();
                var envelope = _typeModel.Deserialize<ProtobufMessageEnvelope<TProtoMessage>>(stream);

                return envelope == null
                    ? throw new SerializationException("The message envelope was not found.")
                    : (SerializerContext)new ProtobufSerializerContext(_typeModel, _objectDeserializer, new ProtoMessageEnvelope<TProtoMessage>(envelope), ContentType);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("An exception occurred while deserializing the message envelope", ex);
            }
        }

        public MessageBody GetMessageBody(string text)
        {
            throw new NotSupportedException("ProtobufMessageDeserializer does not support deserializing from text.");
        }
    }
}
