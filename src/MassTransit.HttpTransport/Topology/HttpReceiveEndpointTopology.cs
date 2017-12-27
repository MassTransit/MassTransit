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
    using GreenPipes;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Filters;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Pipeline.Pipes;
    using MassTransit.Topology;
    using Microsoft.Owin;
    using Specifications;
    using Transport;
    using Transports;


    public class HttpReceiveEndpointTopology :
        IHttpReceiveEndpointTopology
    {
        readonly IConsumePipe _consumePipe;
        readonly IHttpHost _host;
        readonly BusHostCollection<HttpHost> _hosts;
        readonly IPublishTopology _publish;
        readonly Lazy<IPublishEndpointProvider> _publishEndpointProvider;
        readonly IPublishPipe _publishPipe;
        readonly ISendTopology _send;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly ISendPipe _sendPipe;
        readonly Lazy<ISendTransportProvider> _sendTransportProvider;
        readonly IMessageSerializer _serializer;

        public HttpReceiveEndpointTopology(IHttpEndpointConfiguration configuration, Uri inputAddress, IMessageSerializer serializer, IHttpHost host,
            BusHostCollection<HttpHost> hosts)
        {
            InputAddress = inputAddress;
            _serializer = serializer;
            _host = host;
            _hosts = hosts;

            _send = configuration.Topology.Send;
            _publish = configuration.Topology.Publish;

            _consumePipe = configuration.Consume.CreatePipe();
            _sendPipe = configuration.Send.CreatePipe();
            _publishPipe = configuration.Publish.CreatePipe();

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);
            _sendTransportProvider = new Lazy<ISendTransportProvider>(CreateSendTransportProvider);
        }

        public Uri InputAddress { get; }

        public ISendTopology Send => _send;
        public IPublishTopology Publish => _publish;

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider.Value;
        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider.Value;

        public ISendTransportProvider SendTransportProvider => _sendTransportProvider.Value;

        public IReceiveEndpointTopology CreateResponseEndpointTopology(IOwinContext owinContext)
        {
            return new HttpResponseReceiveEndpointTopology(this, owinContext, _sendPipe, _serializer);
        }

        ISendEndpointProvider CreateSendEndpointProvider()
        {
            var provider = new HttpSendEndpointProvider(_serializer, InputAddress, _sendTransportProvider.Value, _sendPipe);

            return new SendEndpointCache(provider);
        }

        IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            return new HttpPublishEndpointProvider(_host, _serializer, _sendTransportProvider.Value, _publishPipe);
        }

        ISendTransportProvider CreateSendTransportProvider()
        {
            var serializerBuilder = new SerializerBuilder();

            IPipe<ReceiveContext> pipe = Pipe.New<ReceiveContext>(x =>
            {
                x.UseFilter(new DeserializeFilter(serializerBuilder.Deserializer, _consumePipe));
            });

            var receivePipe = new ReceivePipe(pipe, _consumePipe);

            return new HttpSendTransportProvider(_hosts, receivePipe, new ReceiveObservable(), this);
        }
    }
}