namespace MassTransit.Tests
{
    using System;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class MessageUrnSpecs
    {
        [Test]
        public void ClosedGenericMessage()
        {
            var urn = MessageUrn.ForType(typeof(G<PingMessage>));
            var expected = new Uri("urn:message:MassTransit.Tests:G[[MassTransit.TestFramework.Messages:PingMessage]]");
            Assert.AreEqual(expected.AbsolutePath, urn.AbsolutePath);
        }

        [Test]
        public void NestedMessage()
        {
            var urn = MessageUrn.ForType(typeof(X));
            Assert.AreEqual(urn.AbsolutePath, "message:MassTransit.Tests:MessageUrnSpecs+X");
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
            Assert.AreEqual(urn.AbsolutePath, "message:MassTransit.TestFramework.Messages:PingMessage");
        }

        [Test]
        public void AttributedMessage()
        {
            var urn = MessageUrn.ForType(typeof(Attributed));
            Assert.AreEqual(urn.AbsolutePath, "message:MyCustomName");
        }

        [Test]
        public void AttributedMessage_with_symbols()
        {
            var urn = MessageUrn.ForType(typeof(AttributedSymbols));
            Assert.AreEqual(urn, "urn:message:\\|,./<>?;'#:@~[]{}¬!\"£$%25^&*()_+`¦€");
        }

        [Test]
        public void AttributedMessage_with_null_throws_error()
        {
            Assert.That(
                () => MessageUrn.ForType(typeof(AttributedNull)),
                Throws.TypeOf<TypeInitializationException>()
                .And.InnerException.TypeOf<ArgumentNullException>()
                .And.InnerException.Message.EqualTo("Value cannot be null. (Parameter 'urn')"));
        }

        [Test]
        public void AttributedMessage_with_empty_throws_error()
        {
            Assert.That(() => MessageUrn.ForType(typeof(AttributedEmpty)),
                Throws.TypeOf<TypeInitializationException>()
                .And.InnerException.TypeOf<ArgumentException>()
                .And.InnerException.Message.EqualTo("Value cannot be empty or whitespace only string. (Parameter 'urn')"));
        }

        [Test]
        public void AttributedMessage_with_whitespace_throws_error()
        {
            Assert.That(() => MessageUrn.ForType(typeof(AttributedWhitespace)),
                Throws.TypeOf<TypeInitializationException>()
                .And.InnerException.TypeOf<ArgumentException>()
                .And.InnerException.Message.EqualTo("Value cannot be empty or whitespace only string. (Parameter 'urn')"));
        }

        [Test]
        public void AttributedMessage_with_default_prefix_throws_error()
        {
            Assert.That(() => MessageUrn.ForType(typeof(AttributedKnownPrefix)),
                Throws.TypeOf<TypeInitializationException>()
                .And.InnerException.TypeOf<ArgumentException>()
                .And.InnerException.Message.EqualTo("Value should not contain the default prefix 'urn:message:'. (Parameter 'urn')"));
        }

        [Test]
        public void AttributedMessage_without_default_prefix()
        {
            var urn = MessageUrn.ForType(typeof(AttributedNoDefaults));
            Assert.AreEqual(urn, "scheme:identifier");
        }

        [Test]
        public void AttributedMessage_without_default_prefix_and_invalid_urn_throws_error()
        {
            Assert.That(() => MessageUrn.ForType(typeof(AttributedNoDefaultsInvalidUrn)),
                Throws.TypeOf<TypeInitializationException>()
                .And.InnerException.TypeOf<UriFormatException>()
                .And.InnerException.Message.EqualTo("'scheme' is not a valid URI."));
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

    [MessageUrn("\\|,./<>?;'#:@~[]{}¬!\"£$%^&*()_+`¦€")]
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
