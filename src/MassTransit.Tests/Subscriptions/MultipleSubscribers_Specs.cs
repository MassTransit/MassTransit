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
namespace MassTransit.Tests.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BusConfigurators;
    using Magnum;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using TestFramework;
    using TextFixtures;

    [TestFixture]
    public class when_multiple_subscribers_to_same_message
        : LoopbackLocalAndRemoteTestFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            RemoteBus.ShouldHaveSubscriptionFor<MyMessage>();

            LocalBus.Publish(new MyMessage());
        }

        List<int> receivedMessages = new List<int>();


        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Subscribe(cf =>
                {
                    cf.Handler<MyMessage>(async message => receivedMessages.Add(1));
                    cf.Handler<MyMessage>(async message => receivedMessages.Add(2));
                });
        }


        [Test]
        public void Each_subscriber_should_only_receive_once()
        {
            ThreadUtil.Sleep(4.Seconds());

            IEnumerable<IGrouping<int, int>> byReceiver = receivedMessages.GroupBy(r => r);
            byReceiver.All(g => g.Count() == 1).ShouldBeTrue();
        }
    }

    public class MyMessage
    {
    }
}