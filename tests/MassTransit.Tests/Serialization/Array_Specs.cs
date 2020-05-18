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
    namespace Array_Specs
    {
        using System;
        using System.Collections.Generic;
        using System.Text;
        using MassTransit.Serialization;
        using NUnit.Framework;
        using Shouldly;


        [TestFixture(typeof(JsonMessageSerializer))]
        public class A_null_array :
            SerializationTest
        {
            [Test]
            public void Should_come_from_json_as_null()
            {
                string source = @"{
  ""messageId"": ""e655000040d800fff4f808d245dca3c8"",
  ""requestId"": """ + _requestId.ToString("N") + @""",
  ""sourceAddress"": ""loopback://localhost/source"",
  ""destinationAddress"": ""loopback://localhost/destination"",
  ""responseAddress"": ""loopback://localhost/response"",
  ""faultAddress"": ""loopback://localhost/fault"",
  ""messageType"": [
    ""urn:message:MassTransit.Tests.Serialization.Array_Specs:SomeArray""
  ],
  ""message"": { ""elements"" : null },
  ""headers"": {}
}";

                var result = Return<SomeArray>(Encoding.UTF8.GetBytes(source));

                result.Elements.ShouldBe(null);
            }

            [Test]
            public void Should_get_one_element()
            {
                string source = @"{
  ""messageId"": ""e655000040d800fff4f808d245dca3c8"",
  ""requestId"": """ + _requestId.ToString("N") + @""",
  ""sourceAddress"": ""loopback://localhost/source"",
  ""destinationAddress"": ""loopback://localhost/destination"",
  ""responseAddress"": ""loopback://localhost/response"",
  ""faultAddress"": ""loopback://localhost/fault"",
  ""messageType"": [
    ""urn:message:MassTransit.Tests.Serialization.Array_Specs:SomeArray""
  ],
  ""message"": {
    ""elements"": 
      {
        ""value"": 27
      }
  },
  ""headers"": {}
}";

                var result = Return<SomeArray>(Encoding.UTF8.GetBytes(source));


                result.Elements.ShouldNotBe(null);
                result.Elements.Length.ShouldBe(1);
            }

            [Test]
            public void Should_return_a_null_array()
            {
                var someArray = new SomeArray();

                SomeArray result = SerializeAndReturn(someArray);

                someArray.Elements.ShouldBe(null);
                result.Elements.ShouldBe(null);
            }

            [Test]
            public void Should_serialize_a_single_element()
            {
                var someArray = new SomeArray();
                someArray.Elements = new ArrayElement[1];
                someArray.Elements[0] = new ArrayElement();
                someArray.Elements[0].Value = 27;

                SomeArray result = SerializeAndReturn(someArray);

                result.Elements.ShouldNotBe(null);
                result.Elements.Length.ShouldBe(1);
            }

            [Test]
            public void Should_serialize_a_single_element_collection()
            {
                var someArray = new SomeCollection();
                someArray.Elements = new ArrayElement[1]
                {
                    new ArrayElement {Value = 27}
                };

                SomeCollection result = SerializeAndReturn(someArray);

                result.Elements.ShouldNotBe(null);
                result.Elements.Count.ShouldBe(1);
            }

            public A_null_array(Type serializerType)
                : base(serializerType)
            {
            }
        }


        class ArrayElement
        {
            public int Value { get; set; }
        }


        class SomeArray
        {
            public ArrayElement[] Elements { get; set; }
        }


        class SomeCollection
        {
            public ICollection<ArrayElement> Elements { get; set; }
        }
    }
}