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
namespace MassTransit.Tests.Services.Subscriptions
{
    using System;
    using MassTransit.Subscriptions;
    using Messages;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class When_Working_With_Subscriptions
    {
       private Uri _testUri;

        [SetUp]
        public void Before_each()
        {
            _testUri = new Uri("msmq://localhost/bob");
        }
        [Test]
        public void Add_Subscription()
        {
            ISubscriptionCache cache = new LocalSubscriptionCache();

            cache.Add(new Subscription(typeof (PingMessage), _testUri));

            Assert.That(cache.List().Count, Is.EqualTo(1));
        }

        [Test]
        public void Add_Subscription_Idempotent()
        {
            int count = 0;
            ISubscriptionCache cache = new LocalSubscriptionCache();
            cache.OnAddSubscription += delegate { count++; };

            cache.Add(new Subscription(typeof (PingMessage), _testUri));
            cache.Add(new Subscription(typeof (PingMessage), _testUri));
            cache.Add(new Subscription(typeof (PongMessage), _testUri));

            Assert.That(cache.List().Count, Is.EqualTo(2));
            Assert.That(count, Is.EqualTo(2));
        }


        [Test]
        public void Event_fires_on_add()
        {
            bool didEventFire = false;
            ISubscriptionCache cache = new LocalSubscriptionCache();
            cache.OnAddSubscription += delegate { didEventFire = true; };

            cache.Add(new Subscription(typeof (PingMessage), _testUri));

            Assert.IsTrue(didEventFire);
        }

        [Test]
        public void Event_fires_on_remove()
        {
            bool didEventFire = false;
            ISubscriptionCache cache = new LocalSubscriptionCache();
            cache.Add(new Subscription(typeof (PingMessage), _testUri));

            cache.OnRemoveSubscription += delegate { didEventFire = true; };
            cache.Remove(new Subscription(typeof (PingMessage), _testUri));

            Assert.IsTrue(didEventFire);
        }

        [Test]
        public void Remove_subscription()
        {
            ISubscriptionCache cache = new LocalSubscriptionCache();
            cache.Add(new Subscription(typeof (PingMessage), _testUri));
            Assert.That(cache.List().Count, Is.EqualTo(1));

            cache.Remove(new Subscription(typeof (PingMessage), _testUri));
            Assert.That(cache.List().Count, Is.EqualTo(0));
        }

        [Test]
        public void Remove_subscription_Idempotent()
        {
            ISubscriptionCache cache = new LocalSubscriptionCache();
            cache.Add(new Subscription(typeof (PingMessage), _testUri));
            Assert.That(cache.List().Count, Is.EqualTo(1));

            cache.Remove(new Subscription(typeof (PingMessage), _testUri));
            cache.Remove(new Subscription(typeof (PingMessage), _testUri));
            Assert.That(cache.List().Count, Is.EqualTo(0));
        }
    }
}