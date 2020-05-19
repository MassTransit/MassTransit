namespace MassTransit.Tests
{
    using System;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class MessageUrnSpecs
    {

        [Test]
        public void SimpleMessage()
        {
            var urn = MessageUrn.ForType(typeof (PingMessage));
            Assert.AreEqual(urn.AbsolutePath, "message:MassTransit.TestFramework.Messages:PingMessage");
        }

        [Test]
        public void NestedMessage()
        {
            var urn = MessageUrn.ForType(typeof (X));
            Assert.AreEqual(urn.AbsolutePath, "message:MassTransit.Tests:MessageUrnSpecs+X");
        }

        [Test]
        public void OpenGenericMessage()
        {
            Assert.That(() => MessageUrn.ForType(typeof(G<>)), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ClosedGenericMessage()
        {
            var urn = MessageUrn.ForType(typeof(G<PingMessage>));
            var expected = new Uri("urn:message:MassTransit.Tests:G[[MassTransit.TestFramework.Messages:PingMessage]]");
            Assert.AreEqual(expected.AbsolutePath,urn.AbsolutePath) ;
        }

        class X{}
    }
    public class G<T>{}
}
