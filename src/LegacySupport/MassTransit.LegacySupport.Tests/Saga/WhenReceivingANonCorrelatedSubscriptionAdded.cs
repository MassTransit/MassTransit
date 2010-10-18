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
namespace MassTransit.Services.LegacyProxy.Tests.Saga
{
    using System;
    using Contexts;
    using Magnum.TestFramework;
    using Messages;
    using NUnit.Framework;
    using ProxyMessages;
    using Rhino.Mocks;
    using MassTransit.TestFramework;

    public class WhenReceivingANonCorrelatedSubscriptionAdded :
        GivenAnInitialSaga
    {
        [When]
        public void MessageReceived()
        {
            var data = new AddSubscription(new Subscription(MessageNameToUse, LegacyClientAddress));
            
            Saga.RaiseEvent(LegacySubscriptionClientSaga.OldSubscriptionAdded, data);
        }

        [Then]
        public void ThenAReceivedOldAddSubscriptionShouldBePublished()
        {
            var message = new ReceivedOldAddSubscription()
            {
                //  CorrelationId = Saga.LegacySubscriptionCorrelationId,
                EndpointUri = LegacyClientAddress,
                MessageName = MessageNameToUse,
            };

            MockBus.AssertWasCalled(bus => bus.Publish(message));
        }
        
    }
}