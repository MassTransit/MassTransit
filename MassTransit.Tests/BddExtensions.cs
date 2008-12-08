namespace MassTransit.Tests
{
    using System;
    using NUnit.Framework;

    public static class BddExtensions
    {
        public static void ShouldBeTrue(this bool value)
        {
            Assert.IsTrue(value);
        }

        public static void ShouldBeFalse(this bool value)
        {
            Assert.IsFalse(value);
        }

		public static void ShouldEqual(this object obj, object expected)
		{
			Assert.AreEqual(expected, obj);
		}
        public static void ShouldEqual(this Uri actual, Uri expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldNotBeNull(this object actual)
        {
            Assert.IsNotNull(actual);
        }
        public static void ShouldBeSameType<T>(this object actual)
        {
            Assert.AreEqual(typeof(T), actual.GetType());
        }
    }
}