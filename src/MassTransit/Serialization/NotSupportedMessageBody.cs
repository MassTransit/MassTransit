namespace MassTransit.Serialization
{
    using System;
    using System.IO;


    public class NotSupportedMessageBody :
        MessageBody
    {
        public long? Length => throw new NotSupportedException();

        public Stream GetStream()
        {
            throw new NotSupportedException();
        }

        public byte[] GetBytes()
        {
            throw new NotSupportedException();
        }

        public string GetString()
        {
            throw new NotSupportedException();
        }
    }
}
