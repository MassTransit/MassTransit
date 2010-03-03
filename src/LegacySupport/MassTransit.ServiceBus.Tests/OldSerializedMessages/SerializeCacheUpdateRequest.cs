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
    using System.IO;
    using NUnit.Framework;
    using Subscriptions.Messages;

    [TestFixture]
    public class SerializeCacheUpdateRequest :
        TestSerialization
    {
        string _pathToFile = @".\OldSerializedMessages\CacheUpdateRequest.txt";

        [Test]
        public void NewToOld()
        {
            var oldMsg = new OldCacheUpdateRequest(new Uri("http://bob/phil"));
            var oldold = Factory.ConvertToOldCacheUpdateRequest(oldMsg);
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

                    StreamAssert.AreEqual(oldStream, newStream);
                }
            }
        }

        [Test]
        public void OldToNew()
        {
            OldCacheUpdateRequest oldMsg;
            using (var str = File.OpenRead(_pathToFile))
            {
                var o = PlainFormatter.Deserialize(str);
                oldMsg = Factory.ConvertToNewCacheUpdateRequest(o);
            }
            Assert.AreEqual(new Uri("http://bob/phil"), oldMsg.RequestingUri);
        }
    }
}