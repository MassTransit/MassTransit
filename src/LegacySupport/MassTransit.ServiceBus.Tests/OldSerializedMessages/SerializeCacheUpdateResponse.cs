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
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using AutoMapper;
    using NUnit.Framework;
    using Subscriptions;
    using Subscriptions.Messages;

    [TestFixture]
    public class SerializeCacheUpdateResponse :
        TestSerialization
    {
        string _pathToFile = @".\OldSerializedMessages\CacheUpdateResponse.txt";

        [Test,Ignore]
        public void NewToOld() //strong to weak
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

                    if(File.Exists(".\\my_msg.txt")) File.Delete(".\\my_msg.txt");
                    using(var fs = File.OpenWrite(".\\my_msg.txt"))
                    {
                        fs.Write(newStream.ToArray(), 0, newStream.ToArray().Length);
                    }
                    StreamAssert.AreEqual(oldStream, newStream);
                }
            }
        }

        [Test,Ignore]
        public void bob()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

            //get an instance of the old object. 
            //set the properties

            //serialize and test
            //can auto mapper help?
            Console.WriteLine(typeof(MassTransit.ServiceBus));
            using (var stream = File.OpenRead(_pathToFile))
            {
                var deserialize = new BinaryFormatter().Deserialize(stream);

                var weakCacheUpdateResponse = Type.GetType("MassTransit.ServiceBus.Subscriptions.Messages.CacheUpdateResponse, MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null");
                var weakSubscriptionType = Type.GetType("MassTransit.ServiceBus.Subscriptions.Subscription, MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null");
                var weakIListSubscriptionType = typeof(IList<>).MakeGenericType(weakSubscriptionType);
                var weakListSubscriptionType = typeof(List<>).MakeGenericType(weakSubscriptionType);

                Mapper.CreateMap(weakSubscriptionType, typeof(Subscription));
                Mapper.CreateMap(weakListSubscriptionType, typeof(List<Subscription>));
                Mapper.CreateMap(weakIListSubscriptionType, typeof(IList<Subscription>));
                Mapper.CreateMap(weakCacheUpdateResponse,typeof(OldCacheUpdateResponse));
                
                var o = (OldCacheUpdateResponse)Mapper.Map(deserialize, weakCacheUpdateResponse, typeof(OldCacheUpdateResponse));
                Console.WriteLine(o.Subscriptions.Count);
            }
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name != "MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null")
                return null;
            var assembly = Assembly.GetAssembly(typeof(LegacySubscriptionProxyService));
            var resourceStream = assembly.GetManifestResourceStream("MassTransit.LegacySupport.OldDll.MassTransit.ServiceBus.dll");
            var buffer = new byte[resourceStream.Length];
            int read = 0;
            while (read < resourceStream.Length)
            {
                read = resourceStream.Read(buffer, read, buffer.Length - read);
            }
            return Assembly.Load(buffer);
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


        [Test]
        public void ArraySerialization()
        {
            List<int> bob = new List<int>();

        }
    }
}