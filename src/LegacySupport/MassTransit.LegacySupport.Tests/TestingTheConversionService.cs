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
namespace MassTransit.Services.LegacyProxy.Tests
{
    using System;
    using System.Threading;
    using MassTransit.Configuration;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Serialization;
    using Transports.Msmq;

    public class TestingTheConversionService
    {
        [Test]
        [Explicit("Hand held testing")]
        public void Bob()
        {
            var mockConfig = MockRepository.GenerateStub<Configuration>();
            mockConfig.Stub(c => c.LegacyServiceProxyUri).Return(new Uri("msmq://localhost/mt_pubsub"));
            mockConfig.Stub(c => c.LegacyServiceDataUri).Return(new Uri("msmq://localhost/mt_legacy"));

            LegacySubscriptionProxyService.SetupAssemblyRedirectForOldMessages();

            IEndpointFactory ef = EndpointFactoryConfigurator.New(c =>
                                                                  {
                                                                      c.RegisterTransport<MsmqEndpoint>();
                                                                      c.SetDefaultSerializer<BinaryMessageSerializer>();
                                                                  });
            var cs = new ConversionService(ef, mockConfig);
            cs.Start();
            Thread.Sleep(1000);
        }
    }
}