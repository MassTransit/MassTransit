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
namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using Builders;
    using EndpointSpecifications;


    public class InMemoryReceiveEndpointSpecification :
        ReceiveEndpointSpecification,
        IInMemoryReceiveEndpointConfigurator,
        IInMemoryReceiveEndpointSpecification
    {
        readonly Uri _baseAddress;
        readonly string _queueName;
        readonly ISendTransportProvider _sendTransportProvider;
        readonly IInMemoryEndpointConfiguration _configuration;
        IPublishEndpointProvider _publishEndpointProvider;
        ISendEndpointProvider _sendEndpointProvider;
        int _transportConcurrencyLimit;

        public InMemoryReceiveEndpointSpecification(Uri baseAddress, string queueName, ISendTransportProvider sendTransportProvider, IInMemoryEndpointConfiguration configuration)
            : base(configuration)
        {
            _baseAddress = baseAddress;
            _queueName = queueName;
            _sendTransportProvider = sendTransportProvider;
            _configuration = configuration;
            _transportConcurrencyLimit = 0;
        }

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider;

        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider;

        public int TransportConcurrencyLimit
        {
            set { _transportConcurrencyLimit = value; }
        }

        public void Apply(IInMemoryBusBuilder builder)
        {
            var inMemoryBusBuilder = builder as InMemoryBusBuilder;
            if (inMemoryBusBuilder == null)
                throw new ArgumentException("The bus builder is expected to by an InMemoryBusBuilder", nameof(inMemoryBusBuilder));

            var inMemoryHost = inMemoryBusBuilder.InMemoryHost as InMemoryHost;
            if (inMemoryHost == null)
                throw new ConfigurationException("Must be an InMemoryHost");

            var receiveEndpointBuilder = new InMemoryReceiveEndpointBuilder(builder, inMemoryHost, _sendTransportProvider, _configuration);

            var receivePipe = CreateReceivePipe(receiveEndpointBuilder);

            var receiveEndpointTopology = receiveEndpointBuilder.CreateReceiveEndpointTopology(InputAddress);

            _sendEndpointProvider = receiveEndpointTopology.SendEndpointProvider;
            _publishEndpointProvider = receiveEndpointTopology.PublishEndpointProvider;

            var transport = inMemoryBusBuilder.InMemoryHost.GetReceiveTransport(_queueName, _transportConcurrencyLimit, receiveEndpointTopology);

            inMemoryHost.ReceiveEndpoints.Add(_queueName, new ReceiveEndpoint(transport, receivePipe));
        }

        protected override Uri GetErrorAddress()
        {
            return new Uri(_baseAddress, $"{_queueName}_error");
        }

        protected override Uri GetDeadLetterAddress()
        {
            return new Uri(_baseAddress, $"{_queueName}_skipped");
        }

        protected override Uri GetInputAddress()
        {
            return new Uri(_baseAddress, $"{_queueName}");
        }
    }
}