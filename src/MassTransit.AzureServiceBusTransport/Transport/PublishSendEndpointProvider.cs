// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Pipes;
    using Policies;
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

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            IServiceBusHost host;
            if (!TryGetMatchingHost(address, out host))
                throw new EndpointNotFoundException("The endpoint address specified an unknown host: " + address);

            return host.RetryPolicy.Retry<ISendEndpoint>(async () =>
            {
                var topicDescription = await host.RootNamespaceManager.CreateTopicSafeAsync(address.GetTopicDescription()).ConfigureAwait(false);

                var messagingFactory = await host.MessagingFactory.ConfigureAwait(false);

                var topicClient = messagingFactory.CreateTopicClient(topicDescription.Path);

                var client = new TopicSendClient(topicClient);

                var sendTransport = new ServiceBusSendTransport(client, host.Supervisor);

                sendTransport.ConnectSendObserver(_sendObservable);

                return new SendEndpoint(sendTransport, _serializer, address, _sourceAddress, SendPipe.Empty);
            });
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservable.Connect(observer);
        }

        bool TryGetMatchingHost(Uri address, out IServiceBusHost host)
        {
            host = _hosts
                .Where(
                    x =>
                        x.Settings.ServiceUri.GetLeftPart(UriPartial.Authority).Equals(address.GetLeftPart(UriPartial.Authority),
                            StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => address.AbsolutePath.StartsWith(x.Settings.ServiceUri.AbsolutePath, StringComparison.OrdinalIgnoreCase) ? 1 : 0)
                .FirstOrDefault();

            return host != null;
        }
    }
}