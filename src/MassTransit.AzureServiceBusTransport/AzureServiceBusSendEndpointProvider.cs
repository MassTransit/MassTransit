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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;
    using Serialization;
    using Transports;


    public class AzureServiceBusSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly ServiceBusHostSettings[] _hosts;
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;

        public AzureServiceBusSendEndpointProvider(IMessageSerializer serializer, Uri sourceAddress, ServiceBusHostSettings[] hosts)
        {
            _hosts = hosts;
            _sourceAddress = sourceAddress;
            _serializer = serializer;
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            ServiceBusHostSettings host =_hosts
                .Where(x => x.ServiceUri.Host.Equals(address.Host, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            if (host == null)
                throw new EndpointNotFoundException("The endpoint address specified an unknown host: " + address);

            var messagingFactory = host.GetMessagingFactory();

            MessageSender messageSender = await messagingFactory.CreateMessageSenderAsync(address.AbsolutePath.Split(new char[]{'/'}).Last());

            var sendTransport = new AzureServiceBusSendTransport(messageSender);

            return new SendEndpoint(sendTransport, _serializer, address, _sourceAddress);
        }
    }
}