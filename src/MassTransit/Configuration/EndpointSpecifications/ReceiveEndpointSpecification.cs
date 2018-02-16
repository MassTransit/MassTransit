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
namespace MassTransit.EndpointSpecifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using GreenPipes.Validation;
    using Pipeline;
    using Pipeline.Filters;
    using Pipeline.Pipes;


    public abstract class ReceiveEndpointSpecification :
        ReceiveSpecification
    {
        readonly IBuildPipeConfigurator<ReceiveContext> _deadLetterPipeConfigurator;
        readonly IBuildPipeConfigurator<ExceptionReceiveContext> _errorPipeConfigurator;
        readonly Lazy<Uri> _inputAddress;
        readonly IList<string> _lateConfigurationKeys;

        protected ReceiveEndpointSpecification(IEndpointConfiguration configuration)
            : base(configuration)
        {
            _lateConfigurationKeys = new List<string>();

            _inputAddress = new Lazy<Uri>(GetInputAddress);

            _deadLetterPipeConfigurator = new PipeConfigurator<ReceiveContext>();
            _errorPipeConfigurator = new PipeConfigurator<ExceptionReceiveContext>();
        }

        public Uri InputAddress => _inputAddress.Value;

        protected virtual void Changed(string key)
        {
            if (IsAlreadyConfigured())
                _lateConfigurationKeys.Add(key);
        }

        protected virtual bool IsAlreadyConfigured()
        {
            return _inputAddress.IsValueCreated;
        }

        protected IReceivePipe CreateReceivePipe(IReceiveEndpointBuilder builder)
        {
            foreach (var specification in Specifications)
                specification.Configure(builder);

            _deadLetterPipeConfigurator.UseFilter(new DeadLetterTransportFilter());
            ReceivePipeConfigurator.UseDeadLetter(_deadLetterPipeConfigurator.Build());

            _errorPipeConfigurator.UseFilter(new GenerateFaultFilter());
            _errorPipeConfigurator.UseFilter(new ErrorTransportFilter());
            ReceivePipeConfigurator.UseRescue(_errorPipeConfigurator.Build());

            ReceivePipeConfigurator.UseFilter(new DeserializeFilter(builder.MessageDeserializer, builder.ConsumePipe));

            return new ReceivePipe(ReceivePipeConfigurator.Build(), builder.ConsumePipe);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return Specifications.SelectMany(x => x.Validate())
                .Concat(Configuration.Validate())
                .Concat(_lateConfigurationKeys.Select(x =>
                    new ConfigurationValidationResult(ValidationResultDisposition.Failure, x, "was configured after being used")));
        }

        protected abstract Uri GetInputAddress();
    }
}