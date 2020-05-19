// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MassTransit.Internals.Extensions;
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