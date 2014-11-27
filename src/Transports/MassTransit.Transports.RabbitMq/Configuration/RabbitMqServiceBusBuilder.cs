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
namespace MassTransit.Transports.RabbitMq.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Serialization;


    public class RabbitMqServiceBusBuilder :
        IServiceBusBuilder
    {
        readonly IEnumerable<RabbitMqHostSettings> _hosts;
        readonly PublishSettings _publishSettings;
        readonly IList<IReceiveEndpoint> _receiveEndpoints;
        IMessageDeserializer _messageDeserializer;
        ISendMessageSerializer _messageSerializer;

        public RabbitMqServiceBusBuilder(IEnumerable<RabbitMqHostSettings> hosts, PublishSettings publishSettings)
        {
            _hosts = hosts;
            _publishSettings = publishSettings;
            _receiveEndpoints = new List<IReceiveEndpoint>();

            _messageDeserializer = new JsonMessageDeserializer(JsonMessageSerializer.Deserializer);
            _messageSerializer = new JsonSendMessageSerializer(JsonMessageSerializer.Serializer);
        }

        public ISendMessageSerializer MessageSerializer
        {
            get { return _messageSerializer; }
        }

        public void AddReceiveEndpoint(IReceiveEndpoint receiveEndpoint)
        {
            _receiveEndpoints.Add(receiveEndpoint);
        }

        public IMessageDeserializer MessageDeserializer
        {
            get { return _messageDeserializer; }
        }

        public IBusControl Build()
        {
            var inboundPipe = new ConsumePipe();

            ISendEndpointProvider sendEndpointProvider = new RabbitMqSendEndpointProvider(_hosts, TODO);

            return new SuperDuperServiceBus(new Uri("rabbitmq://localhost"), inboundPipe, sendEndpointProvider, _receiveEndpoints);
        }
    }


    public class RabbitMqSendEndpointProvider : ISendEndpointProvider
    {
        readonly RabbitMqHostSettings[] _hosts;
        readonly ISendMessageSerializer _serializer;

        public RabbitMqSendEndpointProvider(ISendMessageSerializer serializer, RabbitMqHostSettings[] hosts)
        {
            _hosts = hosts;
            _serializer = serializer;
        }

            public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
            {
                RabbitMqHostSettings host = _hosts
                    .Where(x => x.Host.Equals(address.Host, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
                if (host == null)
                    throw new EndpointNotFoundException("The endpoint address specified an unknown host: " + address);


                var connector = new RabbitMqConnector()

                                var sendToTransport = new RabbitMqSendTransport(sendModel, "fast");
                var sendSerializer = new JsonSendMessageSerializer(JsonMessageSerializer.Serializer);
                var sendToEndpoint = new SendEndpoint(sendToTransport, sendSerializer, new Uri("rabbitmq://localhost/speed/fast"));

                return new RabbitMqSendTransport()
                MessageSender messageSender = await host.GetMessagingFactory().CreateMessageSenderAsync(address.AbsolutePath);

                var sendTransport = new AzureServiceBusSendTransport(messageSender);

                return new SendEndpoint(sendTransport, _serializer, address);
            }
        }

    }
}