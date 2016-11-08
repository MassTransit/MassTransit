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
namespace MassTransit.Builders
{
    using System;
    using Pipeline;
    using Pipeline.Pipes;
    using Transports;
    using Transports.InMemory;


    public class InMemoryReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        public InMemoryReceiveEndpointBuilder(IConsumePipe consumePipe, IBusBuilder busBuilder)
            : base(consumePipe, busBuilder)
        {
        }

        public override ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications)
        {
            var sendPipe = CreateSendPipe(specifications);

            var provider = new InMemorySendEndpointProvider(sourceAddress, SendTransportProvider, MessageSerializer, sendPipe);

            return new SendEndpointCache(provider);
        }

        public override IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications)
        {
            var sendEndpointProvider = new InMemorySendEndpointProvider(sourceAddress, SendTransportProvider, MessageSerializer, SendPipe.Empty);

            var sendEndpointCache = new SendEndpointCache(sendEndpointProvider);

            var publishPipe = CreatePublishPipe(specifications);

            return new InMemoryPublishEndpointProvider(sendEndpointCache, SendTransportProvider, publishPipe);
        }
    }
}