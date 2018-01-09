// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Configurators
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using EndpointSpecifications;
    using GreenPipes;
    using Logging;
    using MassTransit.Configurators;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Filters;
    using MassTransit.Pipeline.Pipes;
    using MassTransit.Topology;
    using Specifications;
    using Transport;


    public abstract class BrokeredMessageReceiverSpecification :
        ReceiveSpecification,
        IBrokeredMessageReceiverConfigurator,
        ISpecification
    {
        ILog _log = Logger.Get<BrokeredMessageReceiver>();

        protected BrokeredMessageReceiverSpecification(IServiceBusEndpointConfiguration configuration)
            : base(configuration)
        {
            InputAddress = new Uri("sb://localhost/");
        }

        public Uri InputAddress { get; set; }

        public ILog Log
        {
            get => _log;
            set => _log = value;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;
        }

        public IBrokeredMessageReceiver Build()
        {
            var result = BusConfigurationResult.CompileResults(Validate());

            try
            {
                var builder = new BrokeredMessageReceiverBuilder(Configuration);

                var receivePipe = CreateReceivePipe(builder);

                var topology = CreateReceiveTopology();

                return new BrokeredMessageReceiver(InputAddress, receivePipe, _log, topology);
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred during handler creation", ex);
            }
        }

        IReceivePipe CreateReceivePipe(BrokeredMessageReceiverBuilder builder)
        {
            foreach (var specification in Specifications)
                specification.Configure(builder);

            ReceivePipeConfigurator.UseFilter(new DeserializeFilter(builder.MessageDeserializer, builder.ConsumePipe));

            IPipe<ReceiveContext> receivePipe = ReceivePipeConfigurator.Build();

            return new ReceivePipe(receivePipe, builder.ConsumePipe);
        }

        protected abstract IReceiveEndpointTopology CreateReceiveTopology();
    }
}