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
namespace MassTransit.HttpTransport.Builders
{
    using System;
    using Clients;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Transports;


    public class HttpReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IHttpReceiveEndpointBuilder
    {
        readonly IHttpHost _host;

        public HttpReceiveEndpointBuilder(IHttpHost host, IConsumePipe consumePipe, IBusBuilder busBuilder)
            : base(consumePipe, busBuilder)
        {
            _host = host;
        }

        public ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications)
        {
            var pipe = CreateSendPipe(specifications);

            var provider = new HttpSendEndpointProvider(MessageSerializer, sourceAddress, SendTransportProvider, pipe);

            return new SendEndpointCache(provider, CacheDurationProvider);
        }

        public IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications)
        {
            var publishPipe = CreatePublishPipe(specifications);

            var sendPipe = CreateSendPipe();

            return new HttpPublishEndpointProvider(_host, MessageSerializer, SendTransportProvider, publishPipe, sendPipe);
        }

        public TimeSpan CacheDurationProvider(Uri address)
        {
            return TimeSpan.FromDays(1);
        }
    }
}