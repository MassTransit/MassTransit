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
    using BusConfigurators;
    using EndpointConfigurators;
    using Pipeline;
    using Pipeline.Pipes;
    using Transports;
    using Transports.InMemory;


    public class InMemoryBusBuilder :
        BusBuilder,
        IInMemoryBusBuilder
    {
        readonly string _busQueueName;
        readonly Uri _inputAddress;
        readonly ISendTransportProvider _sendTransportProvider;

        public InMemoryBusBuilder(InMemoryHost inMemoryHost, ISendTransportProvider sendTransportProvider, BusHostCollection<IBusHostControl> hosts,
            IConsumePipeFactory consumePipeFactory, ISendPipeFactory sendPipeFactory, IPublishPipeFactory publishPipeFactory)
            : base(consumePipeFactory, sendPipeFactory, publishPipeFactory, hosts)
        {
            if (inMemoryHost == null)
                throw new ArgumentNullException(nameof(inMemoryHost));
            if (sendTransportProvider == null)
                throw new ArgumentNullException(nameof(sendTransportProvider));

            _busQueueName = GenerateBusQueueName();
            _inputAddress = new Uri($"loopback://localhost/{_busQueueName}");

            InMemoryHost = inMemoryHost;
            _sendTransportProvider = sendTransportProvider;

            inMemoryHost.ReceiveEndpointFactory =  new InMemoryReceiveEndpointFactory(this);
        }

        public IInMemoryHost InMemoryHost { get; }

        public override ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications)
        {
            var sendPipe = CreateSendPipe(specifications);

            var provider = new InMemorySendEndpointProvider(_inputAddress, _sendTransportProvider, MessageSerializer, sendPipe);

            return new SendEndpointCache(provider);
        }

        public override IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications)
        {
            var sendEndpointProvider = new InMemorySendEndpointProvider(_inputAddress, _sendTransportProvider, MessageSerializer, SendPipe.Empty);

            var sendEndpointCache = new SendEndpointCache(sendEndpointProvider);

            var publishPipe = CreatePublishPipe(specifications);

            return new InMemoryPublishEndpointProvider(sendEndpointCache, _sendTransportProvider, publishPipe);
        }

        protected override Uri GetInputAddress()
        {
            return _inputAddress;
        }

        protected override IConsumePipe GetConsumePipe()
        {
            return CreateBusReceiveEndpoint();
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return _sendTransportProvider;
        }

        IConsumePipe CreateBusReceiveEndpoint()
        {
            var busConsumePipe = CreateConsumePipe();

            var busEndpointConfigurator = new InMemoryReceiveEndpointConfigurator(_busQueueName, busConsumePipe);
            busEndpointConfigurator.Apply(this);

            return busConsumePipe;
        }

        static string GenerateBusQueueName()
        {
            return NewId.Next().ToString("NS");
        }
    }
}