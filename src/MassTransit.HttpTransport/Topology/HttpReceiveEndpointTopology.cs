// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
        ReceiveEndpointTopology,
        IHttpReceiveEndpointTopology
    {
        readonly IHttpEndpointConfiguration _configuration;
        readonly IConsumePipe _consumePipe;
        readonly IHttpHost _host;
        readonly BusHostCollection<HttpHost> _hosts;
        readonly Lazy<ISendTransportProvider> _sendTransportProvider;

        public HttpReceiveEndpointTopology(IHttpEndpointConfiguration configuration, Uri inputAddress, IHttpHost host, BusHostCollection<HttpHost> hosts)
            : base(configuration, inputAddress, host.Address)
        {
            _configuration = configuration;
            _host = host;
            _hosts = hosts;

            _consumePipe = configuration.Consume.CreatePipe();

            _sendTransportProvider = new Lazy<ISendTransportProvider>(CreateSendTransportProvider);
        }

        public IReceiveEndpointTopology CreateResponseEndpointTopology(IOwinContext owinContext)
        {
            return new HttpResponseReceiveEndpointTopology(this, owinContext, SendPipe, Serializer);
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            return new SendEndpointProvider(_sendTransportProvider.Value, SendObservers, Serializer, InputAddress, SendPipe);
        }

        protected override IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            return new HttpPublishEndpointProvider(_host, Serializer, _sendTransportProvider.Value, PublishPipe);
        }

        ISendTransportProvider CreateSendTransportProvider()
        {
            IPipe<ReceiveContext> pipe = Pipe.New<ReceiveContext>(x =>
            {
                x.UseFilter(new DeserializeFilter(_configuration.Serialization.Deserializer, _consumePipe));
            });

            var receivePipe = new ReceivePipe(pipe, _consumePipe);

            return new HttpSendTransportProvider(_hosts, receivePipe, new ReceiveObservable(), this);
        }
    }
}