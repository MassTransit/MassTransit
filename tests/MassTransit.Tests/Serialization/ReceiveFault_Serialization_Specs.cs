namespace MassTransit.Tests.Serialization
{
    using System;
    using Events;
    using MassTransit.Serialization;
    using Metadata;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture(typeof(XmlMessageSerializer))]
    [TestFixture(typeof(JsonMessageSerializer))]
    [TestFixture(typeof(SystemTextJsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    public class ReceiveFault_Serialization_Specs :
        SerializationTest
    {
        [Test]
        public void Should_serialize_fault_from_non_serializable_exception()
        {
            Exception ex = null;
            try
            {
                throw new NonSerializableException();
            }
            catch (Exception e)
            {
                ex = new AggregateException(e);
            }

            var fault = new ReceiveFaultEvent(HostMetadataCache.Host, ex, "Foo", Guid.Empty, new string[0]);


            TestCanSerialize(fault);
        }

        [Test]
        public void Should_serialize_fault_from_serializable_exception()
        {
            Exception ex = null;
            try
            {
                throw new Exception("Boom");
            }
            catch (Exception e)
            {
                ex = new AggregateException(e);
            }

            var fault = new ReceiveFaultEvent(HostMetadataCache.Host, ex, "Foo", Guid.Empty, new string[] { });


            TestCanSerialize(fault);
        }

        public ReceiveFault_Serialization_Specs(Type serializerType)
            : base(serializerType)
        {
        }


        public class NonSerializableException : Exception
        {
        }


        void TestCanSerialize(ReceiveFaultEvent fault)
        {
            byte[] bytes = Serialize(fault);
            bytes.Length.ShouldBeGreaterThan(0);
        }
    }
}
