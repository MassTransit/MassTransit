namespace MassTransit
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;


    public class ArrayMessageBody :
        MessageBody
    {
        readonly ArraySegment<byte> _bytes;

        public ArrayMessageBody(ArraySegment<byte> bytes)
        {
            _bytes = bytes;
        }

        public long? Length => _bytes.Count;

        public Stream GetStream()
        {
            return new MemoryStream(_bytes.Array, _bytes.Offset, _bytes.Count, false);
        }

        public byte[] GetBytes()
        {
            return _bytes.ToArray();
        }

        public string GetString()
        {
            return Encoding.UTF8.GetString(_bytes.Array, _bytes.Offset, _bytes.Count);
        }
    }
}
