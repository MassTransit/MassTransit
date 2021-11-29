namespace MassTransit.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internals;
    using NUnit.Framework;


    class SuperTarget
    {
        public static string StaticProp { get; set; }
        public string InstanceProp { get; set; }
    }


    class SubTarget
        : SuperTarget
    {
        public string AnotherProp { get; private set; }
    }


    class PrivateStatics
        : SuperTarget
    {
        static string CanWeGetPrivates { get; set; }
    }


    class StaticsNoGetter
        : SuperTarget
    {
        public static string ZupMan { set; private get; }
    }


    [TestFixture]
    public class when_getting_static_properties
    {
        [Test]
        public void Can_get_even_with_private_getter()
        {
            IEnumerable<PropertyInfo> props = typeof(StaticsNoGetter).GetAllStaticProperties();
            Assert.That(props.Count(), Is.EqualTo(2));
            IEnumerable<string> names = props.Select(x => x.Name);
            CollectionAssert.Contains(names, "ZupMan");
            CollectionAssert.Contains(names, "StaticProp");
        }

        [Test]
        public void Can_get_private_static_properties()
        {
            IEnumerable<PropertyInfo> props = typeof(PrivateStatics).GetAllStaticProperties();
            Assert.That(props.Count(), Is.EqualTo(2));
            IEnumerable<string> names = props.Select(x => x.Name);
            CollectionAssert.Contains(names, "CanWeGetPrivates");
            CollectionAssert.Contains(names, "StaticProp");
        }

        [Test]
        public void Can_get_property_on_stand_alone_class()
        {
            IEnumerable<PropertyInfo> props = typeof(SuperTarget).GetAllStaticProperties();
            Assert.That(props.Count(), Is.EqualTo(1));
            Assert.That(props.First().Name, Is.EqualTo("StaticProp"));
        }

        [Test]
        public void Can_get_single_property_on_super_from_sub()
        {
            IEnumerable<PropertyInfo> props = typeof(SubTarget).GetAllStaticProperties();
            Assert.That(props.Count(), Is.EqualTo(1));
            Assert.That(props.First().Name, Is.EqualTo("StaticProp"));
        }

        [Test]
        public void Can_get_with_no_hierarchy()
        {
            IEnumerable<PropertyInfo> props = typeof(StaticsNoGetter).GetStaticProperties();
            Assert.That(props.Count(), Is.EqualTo(1));
            Assert.That(props.First().Name, Is.EqualTo("ZupMan"));
        }
    }
}
