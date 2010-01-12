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
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;
    using SerializationCustomization;
    using Subscriptions;

    [TestFixture]
    public class ListTest
    {
        [Test]
        public void HowToTest()
        {
            var bf = new BinaryFormatter();
            var ls = new LegacySurrogateSelector();
            ls.AddSurrogate(new LegacySurrogate("mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "System.Collections.Generic.List`1[MassTransit.ServiceBus.Subscriptions.Subscription]", typeof(List<Subscription>)));
            bf.SurrogateSelector = ls;

            var x = new List<Subscription>();
            x.Add(new Subscription("message", new Uri("http://bob/bill")));
            var stream = new MemoryStream();
            bf.Serialize(stream, x);
            stream.Position = 1;
            bf.Deserialize(stream);
        }
    }
}