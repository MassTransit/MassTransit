#nullable enable
namespace MassTransit.Serialization
{
    using System.IO;
    using System.Net.Mime;
    using ProtoBuf.Meta;

    public class ProtobufBodyMessageSerializer : IMessageSerializer
    {
        private readonly ContentType _contentType;
        private readonly ProtobufMessageEnvelope<object> _envelope;
        private readonly RuntimeTypeModel _typeModel;

        public ProtobufBodyMessageSerializer(MessageEnvelope envelope, ContentType contentType, RuntimeTypeModel typeModel)
        {
            _envelope = new ProtobufMessageEnvelope<object>(envelope);
            _contentType = contentType;
            _typeModel = typeModel;
        }

        public ContentType ContentType => _contentType;

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            _envelope.Update(context);

            return new ProtobufMessageBody<T>(context, _typeModel, _envelope as ProtobufMessageEnvelope<T>);
        }

        public void Overlay(object message)
        {
            using (var stream = new MemoryStream())
            {
                _typeModel.Serialize(stream, message);
                stream.Position = 0;
                var overlayMessage = _typeModel.Deserialize(_envelope.Message?.GetType(), stream);
                _envelope.Message = overlayMessage;
            }
        }
    }
}
