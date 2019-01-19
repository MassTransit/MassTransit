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
    using System;
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
        bool _created;

        public ReceivePipeConfiguration()
        {
            _configurator = new PipeConfigurator<ReceiveContext>();
            DeadLetterConfigurator = new PipeConfigurator<ReceiveContext>();
            ErrorConfigurator = new PipeConfigurator<ExceptionReceiveContext>();
        }

        public ISpecification Specification => _configurator;

        public IReceivePipeConfigurator Configurator => this;

        public IBuildPipeConfigurator<ReceiveContext> DeadLetterConfigurator { get; }

        public IBuildPipeConfigurator<ExceptionReceiveContext> ErrorConfigurator { get; }

        public void AddPipeSpecification(IPipeSpecification<ReceiveContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurator.Validate()
                .Concat(DeadLetterConfigurator.Validate())
                .Concat(ErrorConfigurator.Validate());
        }

        IReceivePipe IReceivePipeConfiguration.CreatePipe(IConsumePipe consumePipe, IMessageDeserializer messageDeserializer,
            Action<IPipeConfigurator<ReceiveContext>> configure)
        {
            if (_created)
                throw new ConfigurationException("The ReceivePipeConfiguration can only be used once.");

            if (configure == null)
            {
                DeadLetterConfigurator.UseFilter(new DeadLetterTransportFilter());
                _configurator.UseDeadLetter(DeadLetterConfigurator.Build());

                ErrorConfigurator.UseFilter(new GenerateFaultFilter());
                ErrorConfigurator.UseFilter(new ErrorTransportFilter());

                _configurator.UseRescue(ErrorConfigurator.Build(), x =>
                {
                    x.Ignore<OperationCanceledException>();
                });
            }
            else
            {
                configure(_configurator);
            }

            _configurator.UseFilter(new DeserializeFilter(messageDeserializer, consumePipe));

            _created = true;

            return new ReceivePipe(_configurator.Build(), consumePipe);
        }

        public IPipe<ReceiveContext> Build()
        {
            return _configurator.Build();
        }
    }
}
