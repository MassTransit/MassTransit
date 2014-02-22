// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Magnum;
    using Magnum.Extensions;
    using NUnit.Framework;
    using TestFramework;
    using TextFixtures;


    [TestFixture]
    public class Removing_a_subscription_client :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_not_remove_any_existing_subscriptions()
        {
            RemoteBus.SubscribeHandler<A>(x => { });

            RemoteBus.ShouldHaveSubscriptionFor<A>();
            LocalBus.ShouldHaveRemoteSubscriptionFor<A>();

            RemoteBus.Dispose();

            ThreadUtil.Sleep(2.Seconds());
            LocalBus.ShouldHaveRemoteSubscriptionFor<A>();
        }


        class A
        {
        }
    }
}