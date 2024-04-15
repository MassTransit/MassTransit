namespace MassTransit.Tests
{
    using System;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class MessageUrnSpecs
    {
        [Test]
        public void AttributedMessage()
        {
            var urn = MessageUrn.ForType(typeof(Attributed));
            Assert.That(urn.AbsolutePath, Is.EqualTo("message:MyCustomName"));
        }

        [Test]
        public void AttributedMessage_with_default_prefix_throws_error()
        {
            Assert.That(() => MessageUrn.ForType(typeof(AttributedKnownPrefix)),
                Throws.TypeOf<TypeInitializationException>()
                    .And.InnerException.TypeOf<ArgumentException>()
                    .And.InnerException.Message.StartsWith("Value should not contain the default prefix 'urn:message:'."));
        }

        [Test]
        public void AttributedMessage_with_empty_throws_error()
        {
            Assert.That(() => MessageUrn.ForType(typeof(AttributedEmpty)),
                Throws.TypeOf<TypeInitializationException>()
                    .And.InnerException.TypeOf<ArgumentException>()
                    .And.InnerException.Message.StartsWith("Value cannot be empty or whitespace only string."));
        }

        [Test]
        public void AttributedMessage_with_null_throws_error()
        {
            Assert.That(
                () => MessageUrn.ForType(typeof(AttributedNull)),
                Throws.TypeOf<TypeInitializationException>()
                    .And.InnerException.TypeOf<ArgumentNullException>()
                    .And.InnerException.Message.StartsWith("Value cannot be null."));
        }

        [Test]
        public void AttributedMessage_with_symbols()
        {
            var urn = MessageUrn.ForType(typeof(AttributedSymbols));
            Assert.That(urn, Is.EqualTo(new Uri("urn:message:\\|,./<>?;'#:@~[]{}�!\"�$%25^&*()_+`��")));
            // Assert.That(urn, Is.EqualTo("urn:message:\\|,./<>?;'#:@~[]{}�!\"�$%25^&*()_+`��"));
        }

        [Test]
        public void AttributedMessage_with_whitespace_throws_error()
        {
            Assert.That(() => MessageUrn.ForType(typeof(AttributedWhitespace)),
                Throws.TypeOf<TypeInitializationException>()
                    .And.InnerException.TypeOf<ArgumentException>()
                    .And.InnerException.Message.StartsWith("Value cannot be empty or whitespace only string."));
        }

        [Test]
        public void AttributedMessage_without_default_prefix()
        {
            var urn = MessageUrn.ForTypeString(typeof(AttributedNoDefaults));
            Assert.That(urn, Is.EqualTo("scheme:identifier"));
        }

        [Test]
        public void AttributedMessage_without_default_prefix_and_invalid_urn_throws_error()
        {
            Assert.That(() => MessageUrn.ForType(typeof(AttributedNoDefaultsInvalidUrn)),
                Throws.TypeOf<TypeInitializationException>()
                    .And.InnerException.TypeOf<UriFormatException>()
                    .And.InnerException.Message.EqualTo("Invalid URN: scheme"));
        }

        [Test]
        public void ClosedGenericMessage()
        {
            var urn = MessageUrn.ForType(typeof(G<PingMessage>));
            var expected = new Uri("urn:message:MassTransit.Tests:G[[MassTransit.TestFramework.Messages:PingMessage]]");
            Assert.That(urn.AbsolutePath, Is.EqualTo(expected.AbsolutePath));
        }

        [Test]
        public void NestedMessage()
        {
            var urn = MessageUrn.ForType(typeof(X));
            Assert.That(urn.AbsolutePath, Is.EqualTo("message:MassTransit.Tests:MessageUrnSpecs+X"));
        }

        [Test]
        public void OpenGenericMessage()
        {
            Assert.That(() => MessageUrn.ForType(typeof(G<>)), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void SimpleMessage()
        {
            var urn = MessageUrn.ForType(typeof(PingMessage));
            Assert.That(urn.AbsolutePath, Is.EqualTo("message:MassTransit.TestFramework.Messages:PingMessage"));
        }


        class X
        {
        }
    }


    public class G<T>
    {
    }


    [MessageUrn("MyCustomName")]
    public class Attributed
    {
    }


    [MessageUrn(null)]
    public class AttributedNull
    {
    }


    [MessageUrn("")]
    public class AttributedEmpty
    {
    }


    [MessageUrn("\t\t   ")]
    public class AttributedWhitespace
    {
    }


    [MessageUrn("urn:message:MyCustomName")]
    public class AttributedKnownPrefix
    {
    }


    [MessageUrn("\\|,./<>?;'#:@~[]{}�!\"�$%^&*()_+`��")]
    public class AttributedSymbols
    {
    }


    [MessageUrn("scheme:identifier", useDefaultPrefix: false)]
    public class AttributedNoDefaults
    {
    }


    [MessageUrn("scheme", useDefaultPrefix: false)]
    public class AttributedNoDefaultsInvalidUrn
    {
    }
}
