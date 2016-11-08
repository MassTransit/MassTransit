// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using System.Reflection;
    using GreenPipes.Internals.Reflection;
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