#nullable enable
namespace MassTransit.Serialization
{
    using ProtoBuf.Meta;
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    public class ProtobufMessageBody<TProtoMessage> : MessageBody where TProtoMessage : class
    {
        readonly SendContext<TProtoMessage> _context;
        byte[]? _bytes;
        ProtobufMessageEnvelope<TProtoMessage>? _envelope;
        string? _string;
        readonly RuntimeTypeModel _typeModel;

        public ProtobufMessageBody(SendContext<TProtoMessage> context, RuntimeTypeModel typeModel, ProtobufMessageEnvelope<TProtoMessage>? envelope = null)
        {
            _context = context;
            _typeModel = typeModel;
            _envelope = envelope;
        }

        public long? Length => _bytes?.Length;

        public byte[] GetBytes()
        {
            if (_bytes != null)
                return _bytes;

            try
            {
                var envelope = _envelope ??= new ProtobufMessageEnvelope<TProtoMessage>(_context, _context.Message, MessageTypeCache<TProtoMessage>.MessageTypeNames);
                using var stream = new MemoryStream();
                _typeModel.Serialize(stream, envelope);
                _bytes = stream.ToArray();
                return _bytes;
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize the message", ex);
            }
        }

        public Stream GetStream()
        {
            return new MemoryStream(GetBytes(), false);
        }

        public string GetString()
        {
            if (_string != null)
                return _string;

            _string = Encoding.UTF8.GetString(GetBytes());
            return _string;
        }
    }
}
