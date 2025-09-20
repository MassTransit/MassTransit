namespace MassTransit.Serialization
{
    using System;
    using System.IO;


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

        public long? Length => _memory.Length;

        public Stream GetStream()
        {
            return new MemoryStream(GetBytes(), false);
        }

        public byte[] GetBytes()
        {
            return _bytes ??= _memory.ToArray();
        }

        public string GetString()
        {
        #if !NET6_0_OR_GREATER && !NETSTANDARD2_1
            return _string ??= MessageDefaults.Encoding.GetString(GetBytes());
        #else
            return _string ??= MessageDefaults.Encoding.GetString(_memory.Span);
        #endif
        }
    }
}
