namespace MassTransit.LegacySupport.Tests
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using Subscriptions.Messages;

    [TestFixture]
    public class SerializeCancelUpdates :
        TestSerialization
    {
        private string _pathToFile = @".\OldSerializedMessages\CancelSubscriptionUpdates.txt";

        [Test]
        public void OldToNew()
        {
            OldCancelSubscriptionUpdates oldMsg;
            using (var str = File.OpenRead(_pathToFile))
            {
                oldMsg = (OldCancelSubscriptionUpdates)NewReader.Deserialize(str);
            }
            Assert.AreEqual(new Uri("http://bob/phil"),oldMsg.RequestingUri);
        }

        [Test]
        public void NewToOld()
        {
            var oldMsg = new OldCancelSubscriptionUpdates(new Uri("http://bob/phil"));
            using (var newStream = new MemoryStream())
            {
                NewWriter.Serialize(newStream, oldMsg);

                newStream.Position = 0;

                using (var oldStream = new MemoryStream())
                {
                    using (var str = File.OpenRead(_pathToFile))
                    {
                        var buff = new byte[str.Length];
                        str.Read(buff, 0, buff.Length);
                        oldStream.Write(buff, 0, buff.Length);
                    }

                    StreamAssert.AreEqual(oldStream, newStream);
                }
            }
        }
    }
}