namespace MassTransit.ActiveMqTransport.Transport
{
    using System.IO;
    using Apache.NMS.ActiveMQ;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;


    public class ZlibCompressionPolicy :
        ICompressionPolicy
    {
        public Stream CreateCompressionStream(Stream data)
        {
            return new DeflaterOutputStream(data);
        }

        public Stream CreateDecompressionStream(Stream data)
        {
            return new InflaterInputStream(data);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
