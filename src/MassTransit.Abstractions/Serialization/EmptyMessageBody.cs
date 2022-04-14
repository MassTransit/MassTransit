namespace MassTransit
{
    using System;
    using System.IO;


    public class EmptyMessageBody :
        MessageBody
    {
        public static MessageBody Instance { get; } = new EmptyMessageBody();

        public long? Length => 0;

        public Stream GetStream()
        {
            return new MemoryStream();
        }

        public byte[] GetBytes()
        {
            return Array.Empty<byte>();
        }

        public string GetString()
        {
            return string.Empty;
        }
    }
}
