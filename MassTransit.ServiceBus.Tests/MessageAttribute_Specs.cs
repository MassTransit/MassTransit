// Copyright 2007-2008 The Apache Software Foundation.
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
    using System;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using ServiceBus;

    [TestFixture]
    public class When_a_message_should_expire_after_a_period_of_time
    {
        [ExpiresIn("00:05:00")]
        internal class MyMessage
        {
        }

        [Test]
        public void The_message_should_use_an_expiration_attribute()
        {
            MyMessage message = new MyMessage();

            object[] attributes = message.GetType().GetCustomAttributes(typeof (ExpiresInAttribute), false);

            Assert.That(attributes.Length, Is.GreaterThan(0));

            foreach (ExpiresInAttribute expiresIn in attributes)
            {
                Assert.That(expiresIn.TimeSpan, Is.EqualTo(TimeSpan.FromMinutes(5)));
            }
        }
    }

    [TestFixture]
    public class When_a_message_needs_to_be_reliable
    {
        [Reliable]
        internal class MyMessage
        {
        }

        [Reliable(false)]
        internal class MyOtherMessage
        {
        }

        [Test]
        public void The_reliable_attribute_should_be_able_to_be_false()
        {
            MyOtherMessage message = new MyOtherMessage();

            object[] attributes = message.GetType().GetCustomAttributes(typeof (ReliableAttribute), false);

            Assert.That(attributes.Length, Is.GreaterThan(0));

            foreach (ReliableAttribute reliable in attributes)
            {
                Assert.That(reliable.Enabled, Is.False);
            }
        }

        [Test]
        public void The_reliable_attribute_should_be_specified()
        {
            MyMessage message = new MyMessage();

            object[] attributes = message.GetType().GetCustomAttributes(typeof (ReliableAttribute), false);

            Assert.That(attributes.Length, Is.GreaterThan(0));

            foreach (ReliableAttribute reliable in attributes)
            {
                Assert.That(reliable.Enabled, Is.True);
            }
        }
    }
}