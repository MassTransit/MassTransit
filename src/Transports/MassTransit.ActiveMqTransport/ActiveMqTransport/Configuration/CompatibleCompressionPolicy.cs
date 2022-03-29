using System.IO;

using Apache.NMS.ActiveMQ;

using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace MassTransit.ActiveMqTransport.Configuration
{
    /// <inheritdoc />
    internal class CompatibleCompressionPolicy : ICompressionPolicy
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
