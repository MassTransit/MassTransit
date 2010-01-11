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
namespace MassTransit.LegacySupport.Tests.Proxy
{
    using System;
    using System.Collections.Generic;
    using Contexts;
    using Internal;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Services.Subscriptions.Messages;
    using Services.Subscriptions.Server.Messages;
    using Subscriptions.Messages;

    public class WhenProxyGetsNewSubscription :
        WithStartedProxyService
    {
        readonly Guid _clientId = Guid.NewGuid();
        readonly Uri _dataUri = new Uri("null://local/data");
        Uri _controlUri = new Uri("null://local/control");

        public override void BecauseOf()
        {
            var info = new SubscriptionInformation(_clientId, 0, "", "", _dataUri);
            var message = new SubscriptionAdded {Subscription = info};

            var saga = new LegacySubscriptionClientSaga(_clientId);
            saga.Bus = new NullServiceBus();

            var data = new OldCacheUpdateRequest(_dataUri);

            saga.RaiseEvent(LegacySubscriptionClientSaga.OldCacheUpdateRequested, data);
            IEnumerable<LegacySubscriptionClientSaga> sagas = new List<LegacySubscriptionClientSaga>
                                                              {
                                                                  saga
                                                              };

            MockRepo.Stub(r => r.Where(s => s.CurrentState == LegacySubscriptionClientSaga.Active)).IgnoreArguments().Return(sagas);
            MockEndpointFactory.Expect(e => e.GetEndpoint(saga.Bus.Endpoint.Uri)).Return(saga.Bus.Endpoint);


            Service.Consume(message);
        }

        [Test]
        public void ShouldQueryRepo()
        {
            MockRepo.AssertWasCalled(r => r.Where(x => true), c => c.IgnoreArguments());
        }

        [Test]
        public void ShouldForwardMessage()
        {
            MockEndpointFactory.VerifyAllExpectations();
        }
    }
}