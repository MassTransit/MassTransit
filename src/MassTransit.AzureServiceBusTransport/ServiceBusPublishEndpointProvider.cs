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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using MassTransit.Pipeline;
    using Transports;
    using Util;


    public class ServiceBusPublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly IServiceBusHost _host;
        readonly IMessageNameFormatter _nameFormatter;
        readonly PublishObservable _publishObservable;
        readonly IPublishPipe _publishPipe;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public ServiceBusPublishEndpointProvider(IServiceBusHost host, ISendEndpointProvider sendEndpointProvider, IPublishPipe publishPipe)
        {
            _host = host;
            _sendEndpointProvider = sendEndpointProvider;
            _publishPipe = publishPipe;
            _nameFormatter = host.MessageNameFormatter;
            _publishObservable = new PublishObservable();
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, Guid? correlationId, Guid? conversationId)
        {
            return new PublishEndpoint(sourceAddress, this, _publishObservable, _publishPipe, correlationId, conversationId);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint(Type messageType)
        {
            if (!TypeMetadataCache.IsValidMessageType(messageType))
                throw new MessageException(messageType, "Anonymous types are not valid message types");

            var address = _nameFormatter.GetTopicAddress(_host, messageType);

            return _sendEndpointProvider.GetSendEndpoint(address);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservable.Connect(observer);
        }
    }
}