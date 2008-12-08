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
namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class When_creating_the_service_bus_with_a_MessageQueueEndpoint
    {

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            _mocks = null;
        }

        private MockRepository _mocks;

        [Test]
        public void The_endpoint_Uri_should_match_the_original_endpoint_Uri()
        {
            string endpointName = @"msmq://localhost/test_servicebus";

            MsmqEndpoint defaultEndpoint = new MsmqEndpoint(endpointName);

            ServiceBus serviceBus = new ServiceBus(defaultEndpoint, _mocks.CreateMock<IObjectBuilder>());

            string machineEndpointName = endpointName.Replace("localhost", Environment.MachineName.ToLowerInvariant());

            Assert.That(serviceBus.Endpoint.Uri.AbsoluteUri, Is.EqualTo(machineEndpointName));
        }
    }
}