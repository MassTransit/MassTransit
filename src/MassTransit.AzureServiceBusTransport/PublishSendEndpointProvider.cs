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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Pipes;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    public class PublishSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly IServiceBusHost[] _hosts;
        readonly SendObservable _sendObservable;
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;

        public PublishSendEndpointProvider(IMessageSerializer serializer, Uri sourceAddress, IServiceBusHost[] hosts)
        {
            _hosts = hosts;
            _sourceAddress = sourceAddress;
            _serializer = serializer;
            _sendObservable = new SendObservable();
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            IServiceBusHost host;
            if(!TryGetMatchingHost(address, out host))
                throw new EndpointNotFoundException("The endpoint address specified an unknown host: " + address);

            TopicDescription topicDescription =
                await (await host.RootNamespaceManager.ConfigureAwait(false)).CreateTopicSafeAsync(address.GetTopicDescription()).ConfigureAwait(false);

            MessagingFactory messagingFactory = await host.MessagingFactory.ConfigureAwait(false);

            MessageSender messageSender = await messagingFactory.CreateMessageSenderAsync(topicDescription.Path).ConfigureAwait(false);

            var sendTransport = new ServiceBusSendTransport(messageSender, host.Supervisor);

            sendTransport.ConnectSendObserver(_sendObservable);

            return new SendEndpoint(sendTransport, _serializer, address, _sourceAddress, SendPipe.Empty);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservable.Connect(observer);
        }

        bool TryGetMatchingHost(Uri address, out IServiceBusHost host)
        {
            host = _hosts
                .Where(x => x.Settings.ServiceUri.GetLeftPart(UriPartial.Authority).Equals(address.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => address.AbsolutePath.StartsWith(x.Settings.ServiceUri.AbsolutePath, StringComparison.OrdinalIgnoreCase) ? 1 : 0)
                .FirstOrDefault();

            return host != null;
        }

    }
}