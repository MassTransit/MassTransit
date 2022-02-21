#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;


    public class EncryptMessageBody<T> :
        MessageBody
        where T : class
    {
        readonly MessageBody _clearMessageBody;
        readonly SendContext<T> _context;
        readonly ICryptoStreamProvider _provider;
        byte[]? _bytes;

        public EncryptMessageBody(ICryptoStreamProvider provider, SendContext<T> context, MessageBody clearMessageBody)
        {
            _provider = provider;
            _context = context;
            _clearMessageBody = clearMessageBody;
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
                using (var cryptoStream = _provider.GetEncryptStream(stream, _context))
                {
                    var bytes = _clearMessageBody.GetBytes();

                    cryptoStream.Write(bytes, 0, bytes.Length);
                }

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
            return Convert.ToBase64String(GetBytes());
        }
    }
}
