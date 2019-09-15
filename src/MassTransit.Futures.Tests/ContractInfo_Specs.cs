namespace MassTransit.Tests
{
    using System;
    using Contracts;
    using Initializers.Factories;
    using NUnit.Framework;
    using TestContracts;
    using Util;


    namespace TestContracts
    {
        using System.Collections.Generic;


        public interface SimpleArgument
        {
            string Name { get; }
            int Age { get; }
        }


        public interface OverarchingArgument
        {
            Guid CommandId { get; }

            SimpleArgument Argument { get; }
        }


        public interface ArrayArgument
        {
            int[] Values { get; }
            string[] Strings { get; }
            IList<SimpleArgument> Members { get; }
        }


        public interface DictionaryArgument
        {
            IDictionary<string, object> Values { get; }
            IEnumerable<KeyValuePair<int, string>> Map { get; }
            IReadOnlyDictionary<string, OverarchingArgument> Arguments { get; }
        }


        public interface NestedArgument
        {
            DictionaryArgument Argument { get; }
        }
    }


    [TestFixture]
    public class Generating_a_contract
    {
        [Test]
        public void Should_work()
        {
            var messageInfo = MessageInfoCache.GetMessageInfo<SimpleArgument>();

            Assert.That(messageInfo, Is.Not.Null);

            Assert.That(messageInfo.MessageTypes[0], Is.EqualTo(MessageUrn.ForType(typeof(SimpleArgument)).ToString()));
        }

        [Test]
        public void Should_support_message_property_types()
        {
            var messageInfo = MessageInfoCache.GetMessageInfo<OverarchingArgument>();

            Assert.That(messageInfo, Is.Not.Null);

            Assert.That(messageInfo.MessageTypes[0], Is.EqualTo(MessageUrn.ForType(typeof(OverarchingArgument)).ToString()));

            Assert.That(messageInfo.Properties, Is.Not.Null);
            Assert.That(messageInfo.Properties.Length, Is.EqualTo(2));
            Assert.That(messageInfo.Properties[1].Kind, Is.EqualTo(PropertyKind.Object));
            Assert.That(messageInfo.Properties[1].PropertyType, Is.EqualTo(TypeMetadataCache<SimpleArgument>.ShortName));
        }

        [Test]
        public void Should_support_object_property_types_an_have_object_info()
        {
            var messageInfo = MessageInfoCache.GetMessageInfo<NestedArgument>();

            Assert.That(messageInfo, Is.Not.Null);

            ObjectInfo[] objectInfos = MessageInfoCache.GetMessageObjectInfo(messageInfo);

            Assert.That(objectInfos, Is.Not.Null);
            Assert.That(objectInfos.Length, Is.EqualTo(4));
            Assert.That(objectInfos[0].ObjectType, Is.EqualTo(TypeMetadataCache<SimpleArgument>.ShortName));
            Assert.That(objectInfos[1].ObjectType, Is.EqualTo(TypeMetadataCache<OverarchingArgument>.ShortName));
            Assert.That(objectInfos[2].ObjectType, Is.EqualTo(TypeMetadataCache<DictionaryArgument>.ShortName));
            Assert.That(objectInfos[3].ObjectType, Is.EqualTo(TypeMetadataCache<NestedArgument>.ShortName));
        }

        [Test]
        public void Should_support_looped_property_types_an_have_object_info()
        {
            var messageInfo = MessageInfoCache.GetMessageInfo<ExceptionInfo>();

            Assert.That(messageInfo, Is.Not.Null);

            ObjectInfo[] objectInfos = MessageInfoCache.GetMessageObjectInfo(messageInfo);

            Assert.That(objectInfos, Is.Not.Null);
            Assert.That(objectInfos.Length, Is.EqualTo(1));
            Assert.That(objectInfos[0].ObjectType, Is.EqualTo(TypeMetadataCache<ExceptionInfo>.ShortName));
        }

        [Test]
        public void Should_support_array_property_types()
        {
            var messageInfo = MessageInfoCache.GetMessageInfo<ArrayArgument>();

            Assert.That(messageInfo, Is.Not.Null);

            Assert.That(messageInfo.MessageTypes[0], Is.EqualTo(MessageUrn.ForType(typeof(ArrayArgument)).ToString()));

            Assert.That(messageInfo.Properties, Is.Not.Null);
            Assert.That(messageInfo.Properties.Length, Is.EqualTo(3));
            Assert.That(messageInfo.Properties[0].Kind.HasFlag(PropertyKind.Array), Is.True);
            Assert.That(messageInfo.Properties[0].PropertyType, Is.EqualTo(TypeMetadataCache<int>.ShortName));
            Assert.That(messageInfo.Properties[1].Kind.HasFlag(PropertyKind.Array), Is.True);
            Assert.That(messageInfo.Properties[1].PropertyType, Is.EqualTo(TypeMetadataCache<string>.ShortName));
            Assert.That(messageInfo.Properties[2].Kind.HasFlag(PropertyKind.Array), Is.True);
            Assert.That(messageInfo.Properties[2].PropertyType, Is.EqualTo(TypeMetadataCache<SimpleArgument>.ShortName));
        }

        [Test]
        public void Should_support_dictionary_property_types()
        {
            var messageInfo = MessageInfoCache.GetMessageInfo<DictionaryArgument>();

            Assert.That(messageInfo, Is.Not.Null);

            Assert.That(messageInfo.MessageTypes[0], Is.EqualTo(MessageUrn.ForType(typeof(DictionaryArgument)).ToString()));

            Assert.That(messageInfo.Properties, Is.Not.Null);
            Assert.That(messageInfo.Properties.Length, Is.EqualTo(3));
            Assert.That(messageInfo.Properties[0].Kind.HasFlag(PropertyKind.Dictionary), Is.True);
            Assert.That(messageInfo.Properties[0].KeyType, Is.EqualTo(TypeMetadataCache<string>.ShortName));
            Assert.That(messageInfo.Properties[0].PropertyType, Is.EqualTo(TypeMetadataCache<object>.ShortName));
            Assert.That(messageInfo.Properties[1].Kind.HasFlag(PropertyKind.Dictionary), Is.True);
            Assert.That(messageInfo.Properties[1].KeyType, Is.EqualTo(TypeMetadataCache<int>.ShortName));
            Assert.That(messageInfo.Properties[1].PropertyType, Is.EqualTo(TypeMetadataCache<string>.ShortName));
            Assert.That(messageInfo.Properties[2].Kind.HasFlag(PropertyKind.Dictionary), Is.True);
            Assert.That(messageInfo.Properties[2].KeyType, Is.EqualTo(TypeMetadataCache<string>.ShortName));
            Assert.That(messageInfo.Properties[2].PropertyType, Is.EqualTo(TypeMetadataCache<OverarchingArgument>.ShortName));
        }
    }
}
