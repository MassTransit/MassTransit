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
namespace MassTransit.Transports.InMemory.Builders
{
    using System;
    using MassTransit.Builders;


    public class InMemoryBusBuilder :
        BusBuilder,
        IInMemoryBusBuilder
    {
        readonly InMemoryReceiveEndpointSpecification _busEndpointSpecification;
        readonly IInMemoryEndpointConfiguration _configuration;
        readonly Uri _inputAddress;

        public InMemoryBusBuilder(InMemoryHost inMemoryHost, ISendTransportProvider sendTransportProvider, BusHostCollection<IBusHostControl> hosts,
            IInMemoryEndpointConfiguration configuration)
            : base(hosts, configuration)
        {
            if (inMemoryHost == null)
                throw new ArgumentNullException(nameof(inMemoryHost));
            if (sendTransportProvider == null)
                throw new ArgumentNullException(nameof(sendTransportProvider));

            var busQueueName = GenerateBusQueueName();
            _inputAddress = new Uri(inMemoryHost.Address, $"{busQueueName}");

            InMemoryHost = inMemoryHost;
            _configuration = configuration;

            var busEndpointSpecification = _configuration.CreateNewConfiguration(ConsumePipe);

            _busEndpointSpecification = new InMemoryReceiveEndpointSpecification(inMemoryHost.Address, busQueueName, sendTransportProvider,
                busEndpointSpecification);

            inMemoryHost.ReceiveEndpointFactory = new InMemoryReceiveEndpointFactory(this, sendTransportProvider, configuration);
        }

        public override IPublishEndpointProvider PublishEndpointProvider => _busEndpointSpecification.PublishEndpointProvider;

        public override ISendEndpointProvider SendEndpointProvider => _busEndpointSpecification.SendEndpointProvider;

        public IInMemoryHost InMemoryHost { get; }

        protected override Uri GetInputAddress()
        {
            return _inputAddress;
        }

        protected override void PreBuild()
        {
            _busEndpointSpecification.Apply(this);
        }

        static string GenerateBusQueueName()
        {
            return NewId.Next().ToString("NS");
        }
    }
}