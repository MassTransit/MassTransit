// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_a_message_fails_to_deserialize_properly :
        InMemoryTestFixture
    {
        [Test]
        public void It_should_respond_with_a_serialization_fault()
        {
            Assert.Throws<RequestFaultException>(async () => await _response);
        }

        IRequestClient<PingMessage, PongMessage> _requestClient;
        Task<PongMessage> _response;

        [TestFixtureSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TestTimeout);

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureBus(IInMemoryServiceBusFactoryConfigurator configurator)
        {
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, async context =>
            {
                throw new SerializationException("This is fine, forcing death");
            });
        }
    }
}