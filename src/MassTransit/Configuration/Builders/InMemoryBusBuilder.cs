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
    using System.Collections.Generic;
    using System.Linq;
    using PipeConfigurators;
    using Pipeline;
    using Transports;
    using Transports.InMemory;


    public class InMemoryBusBuilder :
        BusBuilder,
        IInMemoryBusBuilder
    {
        readonly string _busQueueName;
        readonly IBusHost[] _hosts;
        readonly Uri _inputAddress;
        readonly IReceiveTransportProvider _receiveTransportProvider;
        readonly ISendTransportProvider _sendTransportProvider;

        public InMemoryBusBuilder(IReceiveTransportProvider receiveTransportProvider,
            ISendTransportProvider sendTransportProvider, IEnumerable<IBusHost> hosts,
            IEnumerable<IPipeSpecification<ConsumeContext>> endpointPipeSpecifications)
            : base(endpointPipeSpecifications)
        {
            if (receiveTransportProvider == null)
                throw new ArgumentNullException("receiveTransportProvider");
            if (sendTransportProvider == null)
                throw new ArgumentNullException("sendTransportProvider");

            _busQueueName = GenerateBusQueueName();
            _inputAddress = new Uri(string.Format("loopback://localhost/{0}", _busQueueName));

            _receiveTransportProvider = receiveTransportProvider;
            _sendTransportProvider = sendTransportProvider;
            _hosts = hosts.ToArray();
        }

        public IReceiveTransportProvider ReceiveTransportProvider
        {
            get { return _receiveTransportProvider; }
        }

        protected override Uri GetInputAddress()
        {
            return _inputAddress;
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

        protected override IPublishEndpoint CreatePublishEndpoint()
        {
            return new InMemoryPublishEndpoint(SendEndpointProvider, _sendTransportProvider);
        }

        public IBusControl Build()
        {
            IConsumePipe busConsumePipe = CreateBusReceiveEndpoint();

            return new MassTransitBus(_inputAddress, busConsumePipe, SendEndpointProvider, PublishEndpoint, ReceiveEndpoints, _hosts);
        }

        IConsumePipe CreateBusReceiveEndpoint()
        {
            IConsumePipe busConsumePipe = CreateConsumePipe(Enumerable.Empty<IPipeSpecification<ConsumeContext>>());

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