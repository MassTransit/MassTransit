namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.IO;


    public class ServiceBusMessageBody :
        MessageBody
    {
        readonly BinaryData _data;

        public ServiceBusMessageBody(BinaryData data)
        {
            _data = data;
        }

        public long? Length => _data.ToMemory().Length;

        public Stream GetStream()
        {
            return _data.ToStream();
        }

        public byte[] GetBytes()
        {
            return _data.ToArray();
        }

        public string GetString()
        {
            return _data.ToString();
        }
    }
}
