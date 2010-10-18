// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Services.LegacyProxy.Tests.Proxy
{
    using System;
    using System.Collections.Generic;
    using Contexts;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using ProxyMessages;
    using Subscriptions.Messages;
    using TestFramework;
    using Rhino.Mocks;

    public class WhenProxyGetsASubscriptionRefresh :
        GivenAStartedProxyService
    {
        IEndpoint _mockEndpointToSendUpdateTo;
        const string oldServiceAddress = "msmq://localhost/some_old_service";
        readonly Uri requstingUri = new Uri(oldServiceAddress);

        [When]
        public void MessageReceived()
        {
            _mockEndpointToSendUpdateTo = MockRepository.GenerateStub<IEndpoint>();

            
            MockEndpointFactory.Stub(f => f.GetEndpoint(requstingUri)).Return(_mockEndpointToSendUpdateTo);

            var saga = new LegacySubscriptionClientSaga();
            saga.Bus = MockBus;
            saga.RaiseEvent(LegacySubscriptionClientSaga.OldCacheUpdateRequested, new CacheUpdateRequest(requstingUri));
            var sagas = new List<LegacySubscriptionClientSaga> {saga};
            MockSagaRepository.Stub(r => r.Where(c => c.CurrentState == LegacySubscriptionClientSaga.Active)).IgnoreArguments().Return(sagas);
            var subs = new List<SubscriptionInformation>
                           {
                               new SubscriptionInformation(Guid.Empty, 0, "bob", "cor", new Uri("A://B"))
                           };
            var data = new SubscriptionRefresh(subs);
            
            Service.Consume(data);
        }

        [Then]
        public void ShouldBeASagaQuery()
        {
            MockSagaRepository.AssertWasCalled(r => r.Where(c => c.CurrentState == LegacySubscriptionClientSaga.Active), o=>o.IgnoreArguments()); //lambda
        }

        [Then]
        public void ShouldSendTheUpdate()
        {
            var subs = new List<ProxyMessages.Subscription>();
            subs.Add(new Subscription("bob","cor",new Uri("A://B")));
            Assert.AreEqual(subs,subs);
            var cur = new CacheUpdateResponse(subs);
            var oldmessage = Factory.ConvertToOldCacheUpdateResponse(cur);

            //TODO: remove the ignore arguments
            _mockEndpointToSendUpdateTo.AssertWasCalled(e=>e.Send(oldmessage), o=>o.IgnoreArguments()); // because its the old message type?
        }
    }
}