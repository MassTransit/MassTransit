#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using Newtonsoft.Json.Linq;


    public class NewtonsoftRawXmlMessageBody<TMessage> :
        MessageBody
        where TMessage : class
    {
        readonly SendContext<TMessage> _context;
        readonly JToken? _message;
        byte[]? _bytes;
        string? _string;

        public NewtonsoftRawXmlMessageBody(SendContext<TMessage> context, JToken? message = null)
        {
            _context = context;
            _message = message;
        }

        public long? Length => _bytes?.Length;

        public Stream GetStream()
        {
            return new MemoryStream(GetBytes(), false);
        }

        public byte[] GetBytes()
        {
            if (_bytes != null)
                return _bytes;

            try
            {
                using var stream = new MemoryStream();

                if (_message != null)
                    NewtonsoftXmlMessageSerializer.Serialize(stream, _message, typeof(JToken));
                else
                    NewtonsoftXmlMessageSerializer.Serialize(stream, _context.Message, typeof(TMessage));

                _bytes = stream.ToArray();

                return _bytes;
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

        public string GetString()
        {
            if (_string != null)
                return _string;

            _string = Encoding.UTF8.GetString(GetBytes());
            return _string;
        }
    }
}
