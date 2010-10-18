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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using Exceptions;
    using Magnum;
    using RabbitMQ.Client;
    using Serialization;

    public static class RabbitMqEndpointFactory
    {
        private static ConnectionFactory _factory = new ConnectionFactory();
        static IProtocol _protocol = Protocols.AMQP_0_8;
       
        public static IEndpoint New(IEndpointAddress address, IMessageSerializer serializer)
        {
            return New(new CreateEndpointSettings(address)
            {
                Serializer = serializer,
            });
        }

        public static IEndpoint New(CreateEndpointSettings settings)
        {
            try
            {
                Guard.AgainstNull(settings.Address, "An address for the endpoint must be specified");
                Guard.AgainstNull(settings.ErrorAddress, "An error address for the endpoint must be specified");
                Guard.AgainstNull(settings.Serializer, "A message serializer for the endpoint must be specified");
                EnsureProtocolIsCorrect(settings.Address.Uri);
                EnsureProtocolIsCorrect(settings.ErrorAddress.Uri);

                var transport = new RabbitMqTransport(settings.Address, GetConnection(settings.Address.Uri));

                var errorSettings = new CreateEndpointSettings(settings.ErrorAddress, settings);
                var errorTransport = new RabbitMqTransport(errorSettings.Address, GetConnection(errorSettings.Address.Uri));

                var endpoint = new RabbitMqEndpoint(settings.Address, settings.Serializer, transport, errorTransport);

                return endpoint;
            }
            catch (Exception ex)
            {
                throw new EndpointException(settings.Address.Uri, "Failed to create NMS endpoint", ex);
            }
        }

        static void EnsureProtocolIsCorrect(Uri address)
        {
            if(address.Scheme != "rabbitmq") 
                throw new EndpointException(address, "Address must start with 'rabbitmq' not '{0}'".FormatWith(address.Scheme));
        }

        private static IConnection GetConnection(Uri address)
        {
            var rabbitMqAddress = new UriBuilder("amqp-{0}-{1}".FormatWith(_protocol.MajorVersion, _protocol.MinorVersion), address.Host, _protocol.DefaultPort).Uri;
            return _factory.CreateConnection(rabbitMqAddress);
        }
    }
}