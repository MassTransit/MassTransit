namespace MassTransit.LegacySupport.Tests
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class UriTests :
        TestSerialization
    {
        private string _pathToFile = @".\OldSerializedMessages\Uri.txt";

        [Test]
        public void OldToOld()
        {
            using(var stream = File.OpenRead(_pathToFile))
            {
                Uri u = (Uri)Old.Deserialize(stream);
                Assert.AreEqual(new Uri("http://bob/phil"), u);
            }
        }

        [Test]
        public void OldToNew()
        {
            using (var stream = File.OpenRead(_pathToFile))
            {
                Uri u = (Uri)NewReader.Deserialize(stream);
                Assert.AreEqual(new Uri("http://bob/phil"), u);
            }
        }
    }
}