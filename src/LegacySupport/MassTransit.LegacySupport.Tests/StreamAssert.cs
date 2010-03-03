namespace MassTransit.LegacySupport.Tests
{
    using System.IO;
    using NUnit.Framework;

    public static class StreamAssert
    {
        public static void AreEqual(Stream expected, Stream actual)
        {
            expected.Position = 0;
            actual.Position = 0;

            Assert.AreEqual(expected.Length, actual.Length, "Stream length doesn't match");

            while(expected.Position < expected.Length)
            {
                Assert.AreEqual(expected.ReadByte(), actual.ReadByte(), "The streams vary at position '{0}'", expected.Position);
            }
        }
    }
}