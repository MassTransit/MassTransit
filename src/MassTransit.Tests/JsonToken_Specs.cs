// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
    using System.IO;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using NUnit.Framework;


    [TestFixture]
    public class Using_a_JsonToken_to_convert_types
    {
        [Test]
        public void Should_support_int()
        {
            using (var reader = new StringReader("27"))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var value = JsonMessageSerializer.Deserializer.Deserialize<int>(jsonReader);

                Assert.AreEqual(27, value);
            }
        }
    }
}