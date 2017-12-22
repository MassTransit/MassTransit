// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Serialization;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_an_encrypted_message_to_an_endpoint_with_a_specific_key :
        RabbitMqTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            ISymmetricKeyProvider keyProvider = new TestSymmetricKeyProvider("secure");

            var streamProvider = new AesCryptoStreamProvider(keyProvider, "default");
            configurator.UseEncryptedSerializer(streamProvider);

            base.ConfigureRabbitMqBus(configurator);
        }

        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage(), context => context.Headers.Set(EncryptedMessageSerializer.EncryptionKeyHeader, "secure"));

            ConsumeContext<PingMessage> received = await _handler;

            Assert.AreEqual(EncryptedMessageSerializer.EncryptedContentType, received.ReceiveContext.ContentType);
        }
    }
}