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
namespace MassTransit.EndpointConfigurators
{
    using System;
    using Builders;
    using Pipeline;
    using Transports;


    public class InMemoryReceiveEndpointConfigurator :
        ReceiveEndpointConfigurator,
        IReceiveEndpointConfigurator,
        IInMemoryBusFactorySpecification
    {
        readonly string _queueName;

        public InMemoryReceiveEndpointConfigurator(string queueName, IConsumePipe consumePipe = null)
            : base(consumePipe)
        {
            _queueName = queueName;
        }

        public void Apply(IInMemoryBusBuilder builder)
        {
            IReceiveTransport transport = builder.ReceiveTransportProvider.GetReceiveTransport(_queueName);

            var receivePipe = CreateReceivePipe(builder, consumePipe => new InMemoryReceiveEndpointBuilder(consumePipe));

            builder.AddReceiveEndpoint(_queueName, new ReceiveEndpoint(transport, receivePipe));
        }

        protected override Uri GetErrorAddress()
        {
            return new Uri($"loopback://localhost/{_queueName}_error");
        }

        protected override Uri GetDeadLetterAddress()
        {
            return new Uri($"loopback://localhost/{_queueName}_skipped");
        }

        public Uri InputAddress => new Uri($"loopback://localhost/{_queueName}");
    }
}