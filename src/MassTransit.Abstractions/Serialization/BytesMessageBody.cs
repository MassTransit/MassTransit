namespace MassTransit
{
    using System.IO;
    using System.Text;


    public class BytesMessageBody :
        MessageBody
    {
        readonly byte[] _bytes;
        string? _string;

        public BytesMessageBody(byte[]? bytes)
        {
            _bytes = bytes ?? [];
        }

        public long? Length => _bytes.Length;

        public Stream GetStream()
        {
            return new MemoryStream(_bytes);
        }

        public byte[] GetBytes()
        {
            return _bytes;
        }

        public string GetString()
        {
            return _string ??= Encoding.UTF8.GetString(_bytes);
        }
    }
}
