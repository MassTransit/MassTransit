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
        [TestFixture(typeof(SystemTextJsonMessageSerializer))]
        public class A_null_array :
            SerializationTest
        {
            [Test]
            public void Should_come_from_json_as_null()
            {
                var source = @"{
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
            public void Should_return_a_null_array()
            {
                var someArray = new SomeArray();

                var result = SerializeAndReturn(someArray);

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

                var result = SerializeAndReturn(someArray);

                result.Elements.ShouldNotBe(null);
                result.Elements.Length.ShouldBe(1);
            }

            [Test]
            public void Should_serialize_a_single_element_collection()
            {
                var someArray = new SomeCollection();
                someArray.Elements = new ArrayElement[1] {new ArrayElement {Value = 27}};

                var result = SerializeAndReturn(someArray);

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
