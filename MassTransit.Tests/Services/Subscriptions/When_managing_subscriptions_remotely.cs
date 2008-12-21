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
    using Exceptions;
    using MassTransit.Services.Subscriptions;
    using MassTransit.Services.Subscriptions.Client;
    using MassTransit.Services.Subscriptions.Messages;
    using MassTransit.Subscriptions;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class When_managing_subscriptions_remotely :
        Specification
    {
        private SubscriptionClient sc;
        private IServiceBus _mockBus;
        private ISubscriptionCache _mockCache;
        private IEndpoint _mockEndpoint;
        private IEndpoint _mockBusEndpoint;
        private readonly Uri uri = new Uri("msmq://localhost/test");
        LocalEndpointHandler _localEndpoints;

        protected override void Before_each()
        {
            _mockBus = DynamicMock<IServiceBus>();
            _mockCache = DynamicMock<ISubscriptionCache>();
            _mockEndpoint = DynamicMock<IEndpoint>();
            _mockBusEndpoint = DynamicMock<IEndpoint>();
            _localEndpoints = new LocalEndpointHandler();

            SetupResult.For(_mockBus.Endpoint).Return(_mockBusEndpoint);
            SetupResult.For(_mockBusEndpoint.Uri).Return(uri);
            
            sc = new SubscriptionClient(_mockBus, _mockEndpoint, _localEndpoints);
        }

        protected override void After_each()
        {
            _mockBusEndpoint = null;
            _mockEndpoint = null;
            _mockCache = null;
            _mockBus = null;
            sc = null;
        }

        [Test]
        public void start()
        {
            using(Record())
            {
                Expect.Call(delegate { _mockEndpoint.Send(new CacheUpdateRequest(uri)); }).IgnoreArguments();
            }
            using (Playback())
            {
                sc.Start();
            }
        }


        [Test]
        [ExpectedException(typeof(EndpointException))]
        public void Test_Endpoint_Detection()
        {
            using (Record())
            {
                
            }
            using (Playback())
            {
                SubscriptionClient sc2 = new SubscriptionClient(_mockBus, _mockBusEndpoint, _localEndpoints);
                sc2.Start();
            }
        }
    }
}