// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using Topology;
    using Transports;
    using Util;


    public class ServiceBusPublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly IServiceBusHost _host;
        readonly PublishObservable _publishObservable;
        readonly IPublishPipe _publishPipe;
        readonly IServiceBusPublishTopology _publishTopology;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public ServiceBusPublishEndpointProvider(IServiceBusHost host, ISendEndpointProvider sendEndpointProvider, IPublishPipe publishPipe,
            IServiceBusPublishTopology publishTopology)
        {
            _host = host;
            _sendEndpointProvider = sendEndpointProvider;
            _publishPipe = publishPipe;
            _publishTopology = publishTopology;
            _publishObservable = new PublishObservable();
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, Guid? correlationId, Guid? conversationId)
        {
            return new PublishEndpoint(sourceAddress, this, _publishObservable, _publishPipe, correlationId, conversationId);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>(T message) where T : class
        {
            IServiceBusMessagePublishTopologyConfigurator<T> messageTopology = _publishTopology.GetMessageTopology<T>();

            Uri publishAddress;
            if (!messageTopology.TryGetPublishAddress(_host.Address, message, out publishAddress))
                throw new PublishException($"An address for publishing message type {TypeMetadataCache<T>.ShortName} was not found.");

            return _sendEndpointProvider.GetSendEndpoint(publishAddress);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservable.Connect(observer);
        }
    }
}