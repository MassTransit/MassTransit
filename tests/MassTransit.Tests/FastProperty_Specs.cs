namespace MassTransit.Tests
{
    using System.Linq;
    using System.Reflection;
    using Internals;
    using NUnit.Framework;


    [TestFixture]
    public class FastProperty_Specs
    {
        [Test]
        public void Should_be_able_to_access_a_private_setter()
        {
            var instance = new PrivateSetter();

            var property = instance
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .First(x => x.Name == "Name");


            var fastProperty = new ReadWriteProperty<PrivateSetter>(property);

            const string expectedValue = "Chris";
            fastProperty.Set(instance, expectedValue);

            Assert.AreEqual(expectedValue, fastProperty.Get(instance));
        }

        [Test]
        public void Should_cache_properties_nicely()
        {
            var cache = new ReadWritePropertyCache<PrivateSetter>(true);

            var instance = new PrivateSetter();

            const string expectedValue = "Chris";
            cache["Name"].Set(instance, expectedValue);

            Assert.AreEqual(expectedValue, instance.Name);
        }


        class PrivateSetter
        {
            public string Name { get; private set; }
        }
    }
}
