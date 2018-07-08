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
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Serialization;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture(typeof(JsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(XmlMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    public class Deserializing_an_enumerable_property :
        SerializationTest
    {
        [Test]
        public void Should_deserialize_the_enumerable_type()
        {
            EnumerableMessageType message = new EnumerableMessageTypeImpl {Items = new[] {new MessageItem {Value = "Frank"}, new MessageItem {Value = "Mary"}}};

            EnumerableMessageType result = SerializeAndReturn(message);

            result.Items.Count().ShouldBe(message.Items.Count());
        }

        public Deserializing_an_enumerable_property(Type serializerType)
            : base(serializerType)
        {
        }
    }


    public interface EnumerableMessageType
    {
        IEnumerable<MessageItem> Items { get; }
    }


    class EnumerableMessageTypeImpl : EnumerableMessageType
    {
        public IEnumerable<MessageItem> Items { get; set; }
    }


    public class MessageItem
    {
        public string Value { get; set; }
    }
}