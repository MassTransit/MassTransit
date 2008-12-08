namespace MassTransit.Tests
{
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

        public static void ShouldEqual(this string actual, string expected)
        {
            Assert.AreEqual(expected, actual);
        }
    }
}