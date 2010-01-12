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
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryFormatter();

                var bill = new Bill();
                bill.Numbers = new []{"1"};
                writer.Serialize(stream, bill);
                stream.Position = 0;

                var reader = new BinaryFormatter();
                var ls = new LegacySurrogateSelector();
                ls.AddSurrogate(new WeakToStrongArraySurrogate<int>("mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "System.String[]"));
                var lb = new WeakToStrongBinder();
                var map = new TypeMap("", typeof (Bill).FullName, typeof (Bob));
                lb.AddMap(map);


                reader.SurrogateSelector = ls;
                reader.Binder = lb;

                var a = (Bob) reader.Deserialize(stream);
                Assert.Contains(1, a.Numbers);
            }
        }
    }

    [Serializable]
    public class Bob
    {
        public Bob()
        {
            Numbers = new int[]{};
        }
        public int[] Numbers { get; set; }
    }

    [Serializable]
    public class Bill
    {
         public Bill()
        {
             Numbers = new string[] {};
        }
        public string[] Numbers { get; set; }
    }
}