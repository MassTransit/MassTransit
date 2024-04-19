namespace MassTransit.Tests.Middleware.Internals
{
    using System.Linq;
    using System.Reflection;
    using MassTransit.Internals;
    using NUnit.Framework;


    [TestFixture]
    public class FastProperty_Specs
    {
        [Test]
        public void Should_be_able_to_access_a_private_setter()
        {
            var instance = new PrivateSetter();

            var property = instance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.Name == "Name")
                .First();


            var fastProperty = new ReadWriteProperty<PrivateSetter>(property);

            const string expectedValue = "Chris";
            fastProperty.Set(instance, expectedValue);

            Assert.That(fastProperty.Get(instance), Is.EqualTo(expectedValue));
        }

        [Test]
        public void Should_cache_properties_nicely()
        {
            var cache = new ReadWritePropertyCache<PrivateSetter>(true);

            var instance = new PrivateSetter();

            const string expectedValue = "Chris";
            cache["Name"].Set(instance, expectedValue);

            Assert.That(instance.Name, Is.EqualTo(expectedValue));
        }


        class PrivateSetter
        {
            public string Name { get; private set; }
        }
    }
}
