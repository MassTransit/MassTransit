namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.IO;


    static class StreamExtensions
    {
        public static byte[] ReadAsBytes(this Stream stream)
        {
            if (stream is MemoryStream memoryStream)
                return memoryStream.ToArray();

            if (!stream.CanRead)
                throw new Exception("Stream is not readable");

            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            using (var memStream = new MemoryStream())
            {
                stream.CopyTo(memStream);

                return memStream.ToArray();
            }
        }
    }
}
