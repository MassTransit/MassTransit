namespace MassTransit.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;
	using System;
	using System.Collections;
	using System.Collections.Generic;
    using NUnit.Framework;

    public static class BddExtensions
    {
        public static void ShouldBeTrue(this bool value)
        {
            Assert.IsTrue(value);
        }
        public static void ShouldBeTrue(this bool actual, string message)
        {
            Assert.IsTrue(actual, message);
        }

        public static void ShouldBeFalse(this bool value)
        {
            Assert.IsFalse(value);
        }

		public static void ShouldEqual(this object obj, object expected)
		{
			Assert.AreEqual(expected, obj);
		}
        public static void ShouldEqual(this object obj, object expected, string message)
        {
            Assert.AreEqual(expected, obj, message);
        }
        public static void ShouldEqual(this Uri actual, Uri expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldBeNull(this object actual)
        {
            Assert.IsNull(actual);
        }
        public static void ShouldNotBeNull(this object actual)
        {
            Assert.IsNotNull(actual);
        }
        public static void ShouldBeSameType<T>(this object actual)
        {
            Assert.AreEqual(typeof(T), actual.GetType());
        }


        public static void ShouldBeEmpty<T>(this ICollection<T> collection)
        {
            Assert.AreEqual(0, collection.Count);
        }
        public static void ShouldNotBeEmpty<T>(this ICollection<T> collection)
        {
            Assert.AreNotEqual(0, collection.Count);
        }
        public static void ShouldBeEmpty(this ICollection collection)
        {
            Assert.IsEmpty(collection);
        }

        public static void ShouldNotBeEmpty(this ICollection collection)
        {
            Assert.IsNotEmpty(collection);
        }
    }
}
