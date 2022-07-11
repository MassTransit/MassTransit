#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using Newtonsoft.Json;


    public class NewtonsoftJsonObjectMessageBody :
        MessageBody
    {
        readonly object _value;
        byte[]? _bytes;
        string? _string;

        public NewtonsoftJsonObjectMessageBody(object value)
        {
            _value = value;
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
                using var writer = new StreamWriter(stream, MessageDefaults.Encoding, 1024, true);
                using var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.None };

                NewtonsoftJsonMessageSerializer.Serializer.Serialize(jsonWriter, _value);

                jsonWriter.Flush();
                writer.Flush();

                _bytes = stream.ToArray();

                return _bytes;
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Object serialization failed", ex);
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
