// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Transports;
    using Transports.InMemory;


    public class InMemoryBusBuilder :
        BusBuilder,
        IInMemoryBusBuilder
    {
        readonly string _busQueueName;
        readonly Uri _inputAddress;
        readonly ISendTransportProvider _sendTransportProvider;

        public InMemoryBusBuilder(IReceiveTransportProvider receiveTransportProvider,
            ISendTransportProvider sendTransportProvider, IBusHostControl[] hosts,
            IConsumePipeSpecification consumePipeSpecification)
            : base(consumePipeSpecification, hosts)
        {
            if (receiveTransportProvider == null)
                throw new ArgumentNullException(nameof(receiveTransportProvider));
            if (sendTransportProvider == null)
                throw new ArgumentNullException(nameof(sendTransportProvider));

            _busQueueName = GenerateBusQueueName();
            _inputAddress = new Uri($"loopback://localhost/{_busQueueName}");

            ReceiveTransportProvider = receiveTransportProvider;
            _sendTransportProvider = sendTransportProvider;
        }

        public IReceiveTransportProvider ReceiveTransportProvider { get; }

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

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            var provider = new InMemorySendEndpointProvider(_inputAddress, _sendTransportProvider, MessageSerializer);

            return new SendEndpointCache(provider);
        }

        protected override IPublishEndpointProvider CreatePublishSendEndpointProvider()
        {
            return new InMemoryPublishEndpointProvider(SendEndpointProvider, _sendTransportProvider);
        }

        IConsumePipe CreateBusReceiveEndpoint()
        {
            IConsumePipe busConsumePipe = CreateConsumePipe();

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