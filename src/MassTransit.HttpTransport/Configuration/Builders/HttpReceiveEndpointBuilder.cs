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
    using Clients;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Transports;


    public class HttpReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IHttpReceiveEndpointBuilder
    {

        public HttpReceiveEndpointBuilder(IConsumePipe consumePipe, IBusBuilder busBuilder)
            : base(consumePipe, busBuilder)
        {
        }

        public override ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications)
        {
            var pipe = CreateSendPipe(specifications);

            var provider = new HttpSendEndpointProvider(MessageSerializer, sourceAddress, SendTransportProvider, pipe);

            return new SendEndpointCache(provider, CacheDurationProvider);
        }

        public override IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications)
        {
            var pipe = CreatePublishPipe(specifications);

            return new HttpPublishEndpointProvider();
        }

        public TimeSpan CacheDurationProvider(Uri address)
        {
            return TimeSpan.FromDays(1);
        }
    }
}