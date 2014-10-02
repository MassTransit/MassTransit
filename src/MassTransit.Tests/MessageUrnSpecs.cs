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
            var urn = new MessageUrn(typeof (FlingPing));
            Assert.AreEqual(urn.AbsolutePath, "message:MassTransit.Tests:FlingPing");
        }

        [Test]
        public void SimpleMessageLoopback()
        {
            LoopbackTestMessage(typeof (FlingPing));
        }

        [Test]
        public void NestedMessage()
        {
            var urn = new MessageUrn(typeof (X));
            Assert.AreEqual(urn.AbsolutePath, "message:MassTransit.Tests:MessageUrnSpecs+X");
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
            var expected = new Uri("urn:message:MassTransit.Tests:G[[]]");
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
            var expected = new Uri("urn:message:MassTransit.Tests:H[[],[]]");
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
            var urn = new MessageUrn(typeof (G<FlingPing>));
            var expected = new Uri("urn:message:MassTransit.Tests:G[[MassTransit.Tests:FlingPing]]");
            Assert.AreEqual(expected.AbsolutePath,urn.AbsolutePath) ;
        }

        [Test]
        public void ClosedGenericMessageLoopback()
        {
            LoopbackTestMessage(typeof (G<FlingPing>));
        }

        [Test]
        public void ClosedGenericTupleMessage()
        {
            var urn = new MessageUrn(typeof (H<FlingPing, FlingPing>));
            var expected = new Uri("urn:message:MassTransit.Tests:H[[MassTransit.Tests:FlingPing],[MassTransit.Tests:FlingPing]]");
            Assert.AreEqual(expected.AbsolutePath, urn.AbsolutePath);
        }

        [Test]
        public void ClosedGenericTupleMessageLoopback()
        {
            LoopbackTestMessage(typeof (H<FlingPing, FlingPing>));
        }

        private static void LoopbackTestMessage(Type messageType)
        {
            var urn = new MessageUrn(messageType);
            var recreatedMessageType = urn.GetType(throwOnError: true, ignoreCase: true);
            Assert.AreEqual(recreatedMessageType, messageType);
        }

        class X{}


    }
    public class FlingPing
    {
    }
    public class G<T> { }
    public class H<T1,T2>{}

}