namespace MassTransit.ActiveMqTransport.Tests
{
    using System.IO;
    using System.Text;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
    using Ionic.Zlib;
    using NUnit.Framework;


    [TestFixture]
    public class Compression_Specs
    {
        [Test]
        public void Should_compress_and_decompress()
        {
            var text = "Interoperability is so much fun!";

            using var output = new MemoryStream();
            using (var compressOutput = new ZlibStream(output, CompressionMode.Compress))
            {
                compressOutput.Write(Encoding.UTF8.GetBytes(text));
                compressOutput.Flush();
            }

            output.Flush();

            var bytes = output.ToArray();

            using var input = new MemoryStream(bytes);
            using var decompressStream = new ZlibStream(input, CompressionMode.Decompress);

            var buffer = new byte[16384];
            var length = decompressStream.Read(buffer, 0, buffer.Length);

            var result = Encoding.UTF8.GetString(buffer, 0, length);

            Assert.That(result, Is.EqualTo(text));
        }

        [Test]
        public void Should_compress_and_decompress_both_ways()
        {
            var text = "Interoperability is so much fun!";

            using var output = new MemoryStream();
            using (var compressOutput = new DeflaterOutputStream(output))
            {
                compressOutput.Write(Encoding.UTF8.GetBytes(text));
                compressOutput.Flush();
            }

            output.Flush();

            var bytes = output.ToArray();

            using var input = new MemoryStream(bytes);
            using var decompressStream = new ZlibStream(input, CompressionMode.Decompress);

            var buffer = new byte[16384];
            var length = decompressStream.Read(buffer, 0, buffer.Length);

            var result = Encoding.UTF8.GetString(buffer, 0, length);

            Assert.That(result, Is.EqualTo(text));
        }

        [Test]
        public void Should_compress_and_decompress_using_supported_library()
        {
            var text = "Interoperability is so much fun!";

            using var output = new MemoryStream();
            using (var compressOutput = new ZlibStream(output, CompressionMode.Compress))
            {
                compressOutput.Write(Encoding.UTF8.GetBytes(text));
                compressOutput.Flush();
            }

            output.Flush();

            var bytes = output.ToArray();

            using var input = new MemoryStream(bytes);
            using var decompressStream = new InflaterInputStream(input);

            var buffer = new byte[16384];
            var length = decompressStream.Read(buffer, 0, buffer.Length);

            var result = Encoding.UTF8.GetString(buffer, 0, length);

            Assert.That(result, Is.EqualTo(text));
        }
    }
}
