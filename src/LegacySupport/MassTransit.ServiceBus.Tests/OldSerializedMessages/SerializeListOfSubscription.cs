namespace MassTransit.LegacySupport.Tests.OldSerializedMessages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;
    using Subscriptions;
    using Subscriptions.Messages;

    [TestFixture]
    public class SerializeListOfSubscription :
        TestSerialization
    {
        string _pathToFile = @".\OldSerializedMessages\ListSubscription.txt";

        [Test]
        public void NewToOld()
        {
            var oldSub = new Subscription("the message", new Uri("http://bob/phil"));
            IList<Subscription> oldSubs = new List<Subscription>();
            oldSubs.Add(oldSub);

            using (var newStream = new MemoryStream())
            {
                NewWriter.Serialize(newStream, oldSubs);

                newStream.Position = 0;

                using (var oldStream = new MemoryStream())
                {
                    using (var str = File.OpenRead(_pathToFile))
                    {
                        var buff = new byte[str.Length];
                        str.Read(buff, 0, buff.Length);
                        oldStream.Write(buff, 0, buff.Length);
                    }

                    if (File.Exists(".\\my_msg_2.txt")) File.Delete(".\\my_msg_2.txt");
                    using (var fs = File.OpenWrite(".\\my_msg_2.txt"))
                    {
                        fs.Write(newStream.ToArray(), 0, newStream.ToArray().Length);
                    }

                    StreamAssert.AreEqual(oldStream, newStream);
                }
            }
        }

        [Test]
        public void OldToNew()
        {
            IList<Subscription> oldMsg;
            using (var str = File.OpenRead(_pathToFile))
            {
                oldMsg = (IList<Subscription>)NewReader.Deserialize(str);
            }
            Assert.AreEqual(new Uri("http://bob/phil"), oldMsg[0].EndpointUri);
        }
    }
}