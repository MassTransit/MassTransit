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
namespace MassTransit.Tests.Serialization
{
    using System;
    using MassTransit.Serialization;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture(typeof(JsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(XmlMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    public class Serializing_a_property_of_type_char :
        SerializationTest
    {
        public class PropertyOfChar
        {
            public char Value { get; set; }
        }


        public class PropertyOfNullableChar
        {
            public char? Value { get; set; }
        }


        public Serializing_a_property_of_type_char(Type serializerType)
            : base(serializerType)
        {
        }

        [Test]
        public void Should_handle_a_missing_nullable_value()
        {
            var obj = new PropertyOfNullableChar();

            PropertyOfNullableChar result = SerializeAndReturn(obj);

            result.Value.ShouldBe(obj.Value);
        }

        [Test]
        public void Should_handle_a_present_nullable_value()
        {
            var obj = new PropertyOfNullableChar {Value = 'A'};

            PropertyOfNullableChar result = SerializeAndReturn(obj);

            result.Value.ShouldBe(obj.Value);
        }

        [Test]
        public void Should_handle_a_present_value()
        {
            var obj = new PropertyOfChar {Value = 'A'};

            PropertyOfChar result = SerializeAndReturn(obj);

            result.Value.ShouldBe(obj.Value);
        }

        [Test]
        public void Should_handle_a_string_null()
        {
            var obj = new PropertyOfChar {Value = '\0'};

            PropertyOfChar result = SerializeAndReturn(obj);

            result.Value.ShouldBe(obj.Value);
        }
    }


    [TestFixture(typeof(JsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(XmlMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    public class Serializing_a_string_with_an_escaped_character :
        SerializationTest
    {
        public class SimpleMessage
        {
            public SimpleMessage(string body)
            {
                Body = body;
            }

            protected SimpleMessage()
            {
            }

            public string Body { get; private set; }
        }


        public Serializing_a_string_with_an_escaped_character(Type serializerType)
            : base(serializerType)
        {
        }

        [Test]
        public void Should_handle_a_missing_nullable_value()
        {
            var obj = new SimpleMessage("");

            SimpleMessage result = SerializeAndReturn(obj);

            result.Body.ShouldBe(obj.Body);
        }
    }
}