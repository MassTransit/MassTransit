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
    using MassTransit.Configuration;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Filters;
    using MassTransit.Pipeline.Pipes;
    using Transport;


    public abstract class MessageReceiverSpecification :
        ReceiveSpecification,
        IReceiverConfigurator,
        ISpecification
    {
        readonly IReceiveEndpointConfiguration _configuration;
        ILog _log = Logger.Get<BrokeredMessageReceiver>();

        protected MessageReceiverSpecification(IReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
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

        protected IReceivePipe CreateReceivePipe()
        {
            var builder = new MessageReceiverBuilder(_configuration);

            foreach (var specification in Specifications)
                specification.Configure(builder);

            ReceivePipeConfigurator.UseFilter(new DeserializeFilter(builder.MessageDeserializer, builder.ConsumePipe));

            IPipe<ReceiveContext> receivePipe = ReceivePipeConfigurator.Build();

            return new ReceivePipe(receivePipe, builder.ConsumePipe);
        }
    }
}