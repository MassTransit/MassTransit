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
namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using Pipeline;
    using Pipeline.Filters;
    using Pipeline.Pipes;


    public class ReceivePipeConfiguration :
        IReceivePipeConfiguration,
        IReceivePipeConfigurator,
        ISpecification
    {
        readonly IBuildPipeConfigurator<ReceiveContext> _configurator;
        readonly IBuildPipeConfigurator<ReceiveContext> _deadLetterPipeConfigurator;
        readonly IBuildPipeConfigurator<ExceptionReceiveContext> _errorPipeConfigurator;
        bool _created;

        public ReceivePipeConfiguration()
        {
            _configurator = new PipeConfigurator<ReceiveContext>();
            _deadLetterPipeConfigurator = new PipeConfigurator<ReceiveContext>();
            _errorPipeConfigurator = new PipeConfigurator<ExceptionReceiveContext>();
        }

        public ISpecification Specification => _configurator;

        public IReceivePipeConfigurator Configurator => this;

        public IReceivePipe CreatePipe(IConsumePipe consumePipe, IMessageDeserializer messageDeserializer)
        {
            if (_created)
                throw new ConfigurationException("The ReceivePipeConfiguration can only be used once.");

            _deadLetterPipeConfigurator.UseFilter(new DeadLetterTransportFilter());
            _configurator.UseDeadLetter(_deadLetterPipeConfigurator.Build());

            _errorPipeConfigurator.UseFilter(new GenerateFaultFilter());
            _errorPipeConfigurator.UseFilter(new ErrorTransportFilter());
            _configurator.UseRescue(_errorPipeConfigurator.Build());

            _configurator.UseFilter(new DeserializeFilter(messageDeserializer, consumePipe));

            _created = true;

            return new ReceivePipe(_configurator.Build(), consumePipe);
        }

        public IPipeConfigurator<ReceiveContext> DeadLetterConfigurator => _deadLetterPipeConfigurator;

        public IPipeConfigurator<ExceptionReceiveContext> ErrorConfigurator => _errorPipeConfigurator;

        public void AddPipeSpecification(IPipeSpecification<ReceiveContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurator.Validate()
                .Concat(_deadLetterPipeConfigurator.Validate())
                .Concat(_errorPipeConfigurator.Validate());
        }
    }
}