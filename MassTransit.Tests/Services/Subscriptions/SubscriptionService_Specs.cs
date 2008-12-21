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
    using TestConsumers;
    using TextFixtures;

    [TestFixture]
    public class When_using_the_subscription_service : 
        LoopbackLocalAndRemoteTestFixture
    {
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);

        [Test]
        public void It_should_startup_properly()
        {
            base.ObjectBuilder.GetInstance<ISubscriptionCache>().List().Count
                .ShouldEqual(0);
        }

        [Test]
        public void A_subscription_should_end_up_on_the_service()
        {
            MonitorSubscriptionCache<PingMessage> monitor = new MonitorSubscriptionCache<PingMessage>(base.ObjectBuilder.GetInstance<ISubscriptionCache>());

            LocalBus.Subscribe<TestMessageConsumer<PingMessage>>();

            monitor.ShouldHaveBeenAdded(_timeout);
        }

        [Test]
        public void A_subscription_should_be_removed_from_the_service()
        {
            MonitorSubscriptionCache<PingMessage> monitor = new MonitorSubscriptionCache<PingMessage>(base.ObjectBuilder.GetInstance<ISubscriptionCache>());

            TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();
            var token = LocalBus.Subscribe(consumer);

            monitor.ShouldHaveBeenAdded(_timeout);

            token();

            monitor.ShouldHaveBeenRemoved(_timeout);
        }
    }
}