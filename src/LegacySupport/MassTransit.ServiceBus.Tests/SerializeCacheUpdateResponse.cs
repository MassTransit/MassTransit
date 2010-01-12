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
namespace MassTransit.LegacySupport.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;
    using Subscriptions;
    using Subscriptions.Messages;

    [TestFixture]
    public class SerializeCacheUpdateResponse :
        TestSerialization
    {
        string _pathToFile = @".\OldSerializedMessages\CacheUpdateResponse.txt";

        [Test]
        public void NewToOld()
        {
            IList<Subscription> subs = new List<Subscription>();
            subs.Add(new Subscription("the_message", new Uri("http://bob/phil")));
            var oldMsg = new OldCacheUpdateResponse(subs);

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

        [Test]
        public void OldToNew()
        {
            OldCacheUpdateResponse oldMsg;
            using (var str = File.OpenRead(_pathToFile))
            {
                oldMsg = (OldCacheUpdateResponse) NewReader.Deserialize(str);
            }
            Assert.AreEqual(new Uri("http://bob/phil"), oldMsg.Subscriptions[0].EndpointUri);
        }
    }
}