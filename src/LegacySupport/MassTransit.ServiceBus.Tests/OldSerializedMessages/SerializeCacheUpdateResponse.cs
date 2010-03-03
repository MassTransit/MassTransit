// Copyright 2007-2008 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.LegacySupport.Tests.OldSerializedMessages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;
    using Subscriptions;
    using Subscriptions.Messages;

    [TestFixture]
    public class SerializeCacheUpdateResponse :
        TestSerialization
    {
        string _pathToFile = @".\OldSerializedMessages\CacheUpdateResponse.txt";

        [Test]
        public void NewToOld() //strong to weak
        {
            IList<Subscription> subs = new List<Subscription>();
            subs.Add(new Subscription("the_message", new Uri("http://bob/phil")));
            var oldMsg = new OldCacheUpdateResponse(subs);
            var oldold = Factory.ConvertToOldCacheUpdateResponse(oldMsg);

            using (var newStream = new MemoryStream())
            {
                PlainFormatter.Serialize(newStream, oldold);

                newStream.Position = 0;

                using (var oldStream = new MemoryStream())
                {
                    using (var str = File.OpenRead(_pathToFile))
                    {
                        var buff = new byte[str.Length];
                        str.Read(buff, 0, buff.Length);
                        oldStream.Write(buff, 0, buff.Length);
                    }

                    if(File.Exists(".\\my_msg.txt")) File.Delete(".\\my_msg.txt");
                    using(var fs = File.OpenWrite(".\\my_msg.txt"))
                    {
                        fs.Write(newStream.ToArray(), 0, newStream.ToArray().Length);
                    }

                    //TODO: Hmmm Weird
                    Assert.AreEqual(oldStream.Length, newStream.Length - 2);
                    //StreamAssert.AreEqual(oldStream, newStream);
                }
            }
        }

        [Test]
        public void OldToNew()
        {
            OldCacheUpdateResponse oldMsg;
            using (var str = File.OpenRead(_pathToFile))
            {
                var o = PlainFormatter.Deserialize(str);
                oldMsg = Factory.ConvertToNewCacheUpdateResponse(o) ;
            }
            Assert.AreEqual(new Uri("http://bob/phil"), oldMsg.Subscriptions[0].EndpointUri);
        }
    }
}