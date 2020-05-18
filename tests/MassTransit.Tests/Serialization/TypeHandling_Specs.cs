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
    using System.IO;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using NUnit.Framework;


    [TestFixture]
    public class Should_ignore_the_type_attribute
    {
        [Test]
        public void When_deserializing_a_json_body_with_types()
        {
            JsonSerializer serializer = JsonMessageSerializer.Deserializer;

            string json = "{\"$type\":\"Command.TestCommand, TestDeserializationWithDummyClasses\",\"Id\":1,\"Name\":\"bob\"}";
            using (var reader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var cmd = serializer.Deserialize<ITestCommand>(jsonReader);
            }
        }


        public interface ITestCommand
        {
            int Id { get; }
            string Name { get; }
        }


        public class TestCommand : ITestCommand
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}