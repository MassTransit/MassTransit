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
namespace MassTransit.LegacySupport.Tests.Contexts
{
    using System;
    using Internal;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Subscriptions.Messages;

    public abstract class WithActiveSaga
    {
        public Guid CorrelationId { get; private set; }
        public Uri CorrelationUri { get; private set; }
        public IServiceBus MockBus { get; private set; }
        public LegacySubscriptionClientSaga Saga { get; private set; }

        [TestFixtureSetUp]
        public void Setup()
        {
            CorrelationUri = new Uri("msmq://bob/fitzgerald");
            CorrelationId = Guid.NewGuid();
            Saga = new LegacySubscriptionClientSaga(CorrelationId);
            MockBus = MockRepository.GenerateStub<IServiceBus>();
            MockBus.Stub(x => x.Endpoint).Return(new NullEndpoint());
            Saga.Bus = MockBus;
            var data = new OldCacheUpdateRequest(CorrelationUri);
            Saga.RaiseEvent(LegacySubscriptionClientSaga.OldCacheUpdateRequested, data);
            BecauseOf();
        }

        public abstract void BecauseOf();
    }
}