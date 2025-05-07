namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;


    public class MemoryMessageBody :
        MessageBody
    {
        readonly ReadOnlyMemory<byte> _memory;
        byte[] _bytes;
        string _string;

        public MemoryMessageBody(ReadOnlyMemory<byte> memory)
        {
            _memory = memory;
        }

        public MemoryMessageBody(in string base64String)
        {
            _memory = Convert.FromBase64String(base64String);
        }

        public long? Length => _memory.Length;

        public Stream GetStream()
        {
            return new MemoryStream(GetBytes(), false);
        }

        public byte[] GetBytes()
        {
            return _bytes ??= MemoryMarshal.TryGetArray(_memory, out ArraySegment<byte> bytes)
                ? bytes.Array
                : _memory.ToArray();
        }

        public string GetString()
        {
            return _string ??= Convert.ToBase64String(_memory.Span);
        }
    }
}
