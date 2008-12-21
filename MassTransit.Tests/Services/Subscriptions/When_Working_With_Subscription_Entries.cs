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
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class When_Working_With_Subscription_Entries
    {

        [SetUp]
        public virtual void Before_Each_Test_In_The_Fixture()
        {
            _serviceBusEndPoint = _mocks.StrictMock<IEndpoint>();

            SetupResult.For(_serviceBusEndPoint.Uri).Return(new Uri(_serviceBusQueueName));

            _mocks.ReplayAll();
        }

        protected MockRepository _mocks = new MockRepository();

        protected IServiceBus _serviceBus;
        protected IEndpoint _serviceBusEndPoint;
        protected string _serviceBusQueueName = @"msmq://localhost/test_servicebus";

        [Test]
        public void Comparing_Two_Entries_Should_Return_True()
        {
            SubscriptionCacheEntry left = new SubscriptionCacheEntry(new Subscription("A", _serviceBusEndPoint.Uri));
            SubscriptionCacheEntry right = new SubscriptionCacheEntry(new Subscription("A", _serviceBusEndPoint.Uri));

            Assert.That(left, Is.EqualTo(right));
        }
    }
}