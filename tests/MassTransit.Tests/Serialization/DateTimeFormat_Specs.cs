// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Serialization
{
    using System;
    using System.IO;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using NUnit.Framework;


    [TestFixture]
    public class DateTimeFormat_Specs
    {
        [Test]
        public void Test()
        {
            string message = @"{ IsoDate: ""1994-11-05T13:15:30Z"" }";

            using (var sw = new StringReader(message))
            using (var jsonReader = new JsonTextReader(sw))
            {
                object obj = JsonMessageSerializer.Deserializer.Deserialize(jsonReader, typeof(MessageWithIsoDate));
                var msg = obj as MessageWithIsoDate;

                Assert.That(msg.IsoDate, Is.EqualTo("1994-11-05T13:15:30Z"));
            }
        }


        class MessageWithIsoDate
        {
            public String IsoDate { get; set; }
        }
    }
}