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
namespace MassTransit.HttpTransport.Configuration.Builders
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Transports;


    public class HttpPublishEndpointProvider : IPublishEndpointProvider
    {
        readonly IHttpHost _host;
        readonly IMessageSerializer _serializer;
        readonly ISendTransportProvider _transportProvider;
        readonly IPublishPipe _publishPipe;
        readonly ISendPipe _sendPipe;
        readonly PublishObservable _publishObservable;

        public HttpPublishEndpointProvider(IHttpHost host, IMessageSerializer serializer,
            ISendTransportProvider transportProvider,
            IPublishPipe publishPipe,
            ISendPipe sendPipe)
        {
            _host = host;
            _serializer = serializer;
            _transportProvider = transportProvider;
            _publishPipe = publishPipe;
            _sendPipe = sendPipe;
            _publishObservable = new PublishObservable();
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservable.Connect(observer);
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, Guid? correlationId = null, Guid? conversationId = null)
        {
            return new PublishEndpoint(sourceAddress, this, _publishObservable, _publishPipe, correlationId, conversationId );
        }

        public async Task<ISendEndpoint> GetPublishSendEndpoint(Type messageType)
        {
            var destinationUri = new Uri("http://localhost");

            var st = await _transportProvider.GetSendTransport(destinationUri);
            var sep = new SendEndpoint(st, _serializer, destinationUri, _host.Address, _sendPipe);
            return sep;
        }
    }
}