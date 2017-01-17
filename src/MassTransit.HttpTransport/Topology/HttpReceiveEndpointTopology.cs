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
namespace MassTransit.HttpTransport.Topology
{
    using System;
    using Clients;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Microsoft.Owin;
    using Transports;
    using Transports.InMemory;
    using Transports.InMemory.Topology;


    public class HttpReceiveEndpointTopology :
        IHttpReceiveEndpointTopology
    {
        readonly IHttpHost _host;
        readonly IPublishTopology _publish;
        readonly Lazy<IPublishEndpointProvider> _publishEndpointProvider;
        readonly IPublishPipe _publishPipe;
        readonly ISendTopology _send;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly ISendPipe _sendPipe;
        readonly ISendTransportProvider _sendTransportProvider;
        readonly IMessageSerializer _serializer;

        public HttpReceiveEndpointTopology(IInMemoryEndpointConfiguration configuration, Uri inputAddress, IMessageSerializer serializer,
            ISendTransportProvider sendTransportProvider, IHttpHost host)
        {
            InputAddress = inputAddress;
            _serializer = serializer;
            _sendTransportProvider = sendTransportProvider;
            _host = host;

            _send = configuration.SendTopology;
            _publish = configuration.PublishTopology;

            _sendPipe = configuration.CreateSendPipe();
            _publishPipe = configuration.CreatePublishPipe();

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);
        }

        public Uri InputAddress { get; }

        public ISendTopology Send => _send;
        public IPublishTopology Publish => _publish;

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider.Value;
        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider.Value;

        public IReceiveEndpointTopology CreateResponseEndpointTopology(IOwinContext owinContext)
        {
            return new HttpResponseReceiveEndpointTopology(this, owinContext, _sendPipe, _serializer);
        }

        ISendEndpointProvider CreateSendEndpointProvider()
        {
            var provider = new HttpSendEndpointProvider(_serializer, InputAddress, _sendTransportProvider, _sendPipe);

            return new SendEndpointCache(provider);
        }

        IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            return new HttpPublishEndpointProvider(_host, _serializer, _sendTransportProvider, _publishPipe);
        }
    }
}