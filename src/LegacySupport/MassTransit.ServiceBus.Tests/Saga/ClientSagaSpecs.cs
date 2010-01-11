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
namespace MassTransit.LegacySupport.Tests.Saga
{
    using Contexts;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Subscriptions.Messages;

    public class ClientSagaSpecs :
        WithActiveSaga
    {
        public override void BecauseOf()
        {
            var data = new OldCancelSubscriptionUpdates(CorrelationUri);
            Saga.RaiseEvent(LegacySubscriptionClientSaga.OldCancelSubscriptionUpdates, data);
        }

        [Test]
        public void StateShouldBeActive()
        {
            Assert.That(Saga.CurrentState, Is.EqualTo(LegacySubscriptionClientSaga.Completed));
        }

        [Test]
        public void MessageShouldBePublished()
        {
            var msg = new LegacySubscriptionClientAdded
                      {
                          ClientId = CorrelationId,
                          ControlUri = MockBus.Endpoint.Uri,
                          DataUri = CorrelationUri
                      };
            MockBus.AssertWasCalled(bus => bus.Publish(msg));
        }
    }
}