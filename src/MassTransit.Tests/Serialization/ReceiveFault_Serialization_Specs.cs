using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Tests.Serialization {
    using Events;
    using MassTransit.Serialization;
    using NUnit.Framework;
    using Shouldly;
    using Util;


    [TestFixture(typeof(XmlMessageSerializer))]
    [TestFixture(typeof(JsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
#if !NETCORE
    [TestFixture(typeof(BinaryMessageSerializer))]
#endif
    public class ReceiveFault_Serialization_Specs :
        SerializationTest {
        public ReceiveFault_Serialization_Specs(Type serializerType)
            : base(serializerType) {
        }

        public class NonSerializableException : Exception
        {

        }

        [Test]
        public void Should_serialize_fault_from_serializable_exception() {

            Exception ex = null;
            try {
                throw new Exception("Boom");
            }
            catch (Exception e) {
                ex = new AggregateException(e);
            }
            var fault = new ReceiveFaultEvent(HostMetadataCache.Host, ex, "Foo", Guid.Empty);


            TestCanSerialize(fault);
        }

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
            var fault = new ReceiveFaultEvent(HostMetadataCache.Host, ex, "Foo", Guid.Empty);


            TestCanSerialize(fault);
        }

        void TestCanSerialize(ReceiveFaultEvent fault) {
            var bytes = Serialize(fault);
            bytes.Length.ShouldBeGreaterThan(0);
        }
    }
}
