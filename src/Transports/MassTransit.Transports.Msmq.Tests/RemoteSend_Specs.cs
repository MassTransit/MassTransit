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
    using MassTransit.Serialization;
    using NUnit.Framework;

    [TestFixture, Category("Integration")]
    public class Sending_to_a_remote_endpoint_that_is_unavailable
    {
        private CreateMsmqEndpointSettings _settings;
        private CreateMsmqTransportSettings _cacheSettings;

        [SetUp]
        public void Setup()
        {
            EstablishContext();
        }

        protected virtual void EstablishContext()
        {
            _settings = new CreateMsmqEndpointSettings("msmq://unknownHost/input_queue")
                {
                    Serializer = new XmlMessageSerializer(),
                    PurgeExistingMessages = true,
                };

            _cacheSettings = MsmqTransportFactory.GetSettingsForRemoteEndpointCache(_settings);

            IEndpoint endpoint = MsmqEndpointFactory.New(_settings);

            endpoint.Send(new SimpleMessage {Name = "Chris"});
        }

        [Test, Ignore]
        public void Should_keep_the_message_in_the_outbound_queue()
        {
            MsmqEndpointManagement.Manage(_cacheSettings.Address, q => { Assert.AreEqual(1, q.Count()); });
        }
    }
}