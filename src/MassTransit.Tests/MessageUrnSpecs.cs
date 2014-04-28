namespace MassTransit.Tests
{
    using System;
    using NUnit.Framework;
    using TestFramework.Examples.Messages;

    [TestFixture]
    public class MessageUrnSpecs
    {
        
        [Test]
        public void SimpleMessage()
        {
            var urn = new MessageUrn(typeof (Ping));
            Assert.AreEqual(urn.AbsolutePath, "message:MassTransit.TestFramework.Examples.Messages:Ping:MassTransit.Tests");
        }

        [Test]
        public void SimpleMessageLoopback()
        {
            LoopbackTestMessage(typeof (Ping));
        }

        [Test]
        public void NestedMessage()
        {
            var urn = new MessageUrn(typeof (X));
            Assert.AreEqual(urn.AbsolutePath, "message:MassTransit.Tests:MessageUrnSpecs+X:MassTransit.Tests");
        }

        [Test]
        public void NestedMessageLoopback()
        {
            LoopbackTestMessage(typeof (X));
        }

        [Test]
        public void OpenGenericMessage()
        {
            var urn = new MessageUrn(typeof (G<>));
            var expected = new Uri("urn:message:MassTransit.Tests:G[[]]:MassTransit.Tests");
            Assert.AreEqual(expected.AbsolutePath, urn.AbsolutePath);
        }

        [Test]
        public void OpenGenericMessageLoopback()
        {
            LoopbackTestMessage(typeof (G<>));
        }

        [Test]
        public void OpenGenericTupleMessage()
        {
            var urn = new MessageUrn(typeof (H<,>));
            var expected = new Uri("urn:message:MassTransit.Tests:H[[],[]]:MassTransit.Tests");
            Assert.AreEqual(expected.AbsolutePath, urn.AbsolutePath);
        }

        [Test]
        public void OpenGenericTupleMessageLoopback()
        {
            LoopbackTestMessage(typeof (H<,>));
        }

        [Test]
        public void ClosedGenericMessage()
        {
            var urn = new MessageUrn(typeof (G<Ping>));
            var expected = new Uri("urn:message:MassTransit.Tests:G[[MassTransit.TestFramework.Examples.Messages:Ping:MassTransit.Tests]]:MassTransit.Tests");
            Assert.AreEqual(expected.AbsolutePath,urn.AbsolutePath) ;
        }

        [Test]
        public void ClosedGenericMessageLoopback()
        {
            LoopbackTestMessage(typeof (G<Ping>));
        }

        [Test]
        public void ClosedGenericTupleMessage()
        {
            var urn = new MessageUrn(typeof (H<Ping, Ping>));
            var expected = new Uri("urn:message:MassTransit.Tests:H[[MassTransit.TestFramework.Examples.Messages:Ping:MassTransit.Tests],[MassTransit.TestFramework.Examples.Messages:Ping:MassTransit.Tests]]:MassTransit.Tests");
            Assert.AreEqual(expected.AbsolutePath, urn.AbsolutePath);
        }

        [Test]
        public void ClosedGenericTupleMessageLoopback()
        {
            LoopbackTestMessage(typeof (H<Ping, Ping>));
        }

        private static void LoopbackTestMessage(Type messageType)
        {
            var urn = new MessageUrn(messageType);
            var recreatedMessageType = urn.GetType(throwOnError: true, ignoreCase: true);
            Assert.AreEqual(recreatedMessageType, messageType);
        }

        class X{}
    }
    public class G<T>{}
    public class H<T1,T2>{}
}