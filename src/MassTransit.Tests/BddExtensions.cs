namespace MassTransit.Tests
{
	using NUnit.Framework;

	public static class BddExtensions
    {
        public static void ShouldBeGreaterThan(this int value, int target)
        {
            Assert.That(value, Is.GreaterThan(target));
        }

        public static void ShouldBeLessThan(this int value, int target)
        {
            Assert.That(value, Is.LessThan(target));
        }
    }
}
