// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Messaging;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using TestFramework;

    [TestFixture]
    public class TransactionalToNonTransactional_Specs
    {
        const string _transactionalUri = "msmq://localhost/transactional";
        const string _nonTransactionalUri = "msmq://localhost/nontransactional";

        public class MyMessage
        {
        }

        [SetUp]
        public void Setup()
        {
            if (MessageQueue.Exists(new MsmqEndpointAddress(new Uri(_transactionalUri)).LocalName))
                MessageQueue.Delete(new MsmqEndpointAddress(new Uri(_transactionalUri)).LocalName);

            if (MessageQueue.Exists(new MsmqEndpointAddress(new Uri(_nonTransactionalUri)).LocalName))
                MessageQueue.Delete(new MsmqEndpointAddress(new Uri(_nonTransactionalUri)).LocalName);

            if (MessageQueue.Exists(new MsmqEndpointAddress(new Uri(_transactionalUri + "_subscriptions")).LocalName))
                MessageQueue.Delete(new MsmqEndpointAddress(new Uri(_transactionalUri + "_subscriptions")).LocalName);

            if (MessageQueue.Exists(new MsmqEndpointAddress(new Uri(_nonTransactionalUri + "_subscriptions")).LocalName))
                MessageQueue.Delete(new MsmqEndpointAddress(new Uri(_nonTransactionalUri + "_subscriptions")).LocalName);
        }

        [Test]
        public void Should_publish_to_non_transactional_queue()
        {
            using (IServiceBus transactionalBus = ServiceBusFactory.New(x =>
                {
                    x.UseMsmq(m => m.UseMulticastSubscriptionClient());
                    x.ReceiveFrom(_transactionalUri);
                    x.SetCreateMissingQueues(true);
                    x.SetCreateTransactionalQueues(true);
                }))
            {
                using (IServiceBus nonTransactionalBus = ServiceBusFactory.New(x =>
                    {
                        x.UseMsmq(m => m.UseMulticastSubscriptionClient());
                        x.ReceiveFrom(_nonTransactionalUri);
                        x.SetCreateMissingQueues(true);
                        x.SetCreateTransactionalQueues(false);
                    }))
                {
                    var future = new Future<MyMessage>();

                    nonTransactionalBus.SubscribeHandler<MyMessage>(future.Complete);

                    transactionalBus.ShouldHaveSubscriptionFor<MyMessage>();

                    transactionalBus.Publish(new MyMessage(),
                        ctx => ctx.IfNoSubscribers(() => { throw new Exception("NoSubscribers"); }));

                    future.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("The published message was not received>");
                }
            }
        }
    }
}