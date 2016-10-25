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
namespace MassTransit.EndpointConfigurators
{
    using System;
    using Builders;
    using Pipeline;
    using Transports;
    using Transports.InMemory;


    public class InMemoryReceiveEndpointConfigurator :
        ReceiveEndpointConfigurator,
        IInMemoryReceiveEndpointConfigurator,
        IInMemoryBusFactorySpecification
    {
        readonly string _queueName;
        int _transportConcurrencyLimit;
        ReceiveEndpoint _receiveEndpoint;

        public InMemoryReceiveEndpointConfigurator(string queueName, IConsumePipe consumePipe = null)
            : base(consumePipe)
        {
            _queueName = queueName;
            _transportConcurrencyLimit = 0;
        }

        public IReceiveEndpoint ReceiveEndpoint => _receiveEndpoint;

        public void Apply(IInMemoryBusBuilder builder)
        {
            var transport = builder.InMemoryHost.GetReceiveTransport(_queueName, _transportConcurrencyLimit);

            var receivePipe = CreateReceivePipe(builder, consumePipe => new InMemoryReceiveEndpointBuilder(consumePipe));

            _receiveEndpoint = new ReceiveEndpoint(transport, receivePipe);

            var inMemoryHost = builder.InMemoryHost as InMemoryHost;
            if (inMemoryHost == null)
                throw new ConfigurationException("Must be an InMemoryHost");

            inMemoryHost.ReceiveEndpoints.Add(_queueName, _receiveEndpoint);
        }

        public int TransportConcurrencyLimit
        {
            set { _transportConcurrencyLimit = value; }
        }

        protected override Uri GetErrorAddress()
        {
            return new Uri($"loopback://localhost/{_queueName}_error");
        }

        protected override Uri GetDeadLetterAddress()
        {
            return new Uri($"loopback://localhost/{_queueName}_skipped");
        }

        protected override Uri GetInputAddress()
        {
            return new Uri($"loopback://localhost/{_queueName}");
        }
    }
}