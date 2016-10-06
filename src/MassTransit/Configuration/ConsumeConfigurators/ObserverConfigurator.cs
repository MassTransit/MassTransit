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
namespace MassTransit.ConsumeConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using PipeConfigurators;


    public class ObserverConfigurator<TMessage> :
        IObserverConfigurator<TMessage>,
        IReceiveEndpointSpecification
        where TMessage : class
    {
        readonly IPipeSpecification<ConsumeContext<TMessage>> _handlerConfigurator;
        readonly IBuildPipeConfigurator<ConsumeContext<TMessage>> _pipeConfigurator;

        public ObserverConfigurator(IObserver<ConsumeContext<TMessage>> observer)
        {
            _pipeConfigurator = new PipeConfigurator<ConsumeContext<TMessage>>();
            _handlerConfigurator = new ObserverPipeSpecification<TMessage>(observer);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            _pipeConfigurator.AddPipeSpecification(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _handlerConfigurator.Validate().Concat(_pipeConfigurator.Validate());
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            _pipeConfigurator.AddPipeSpecification(_handlerConfigurator);

            IPipe<ConsumeContext<TMessage>> pipe = _pipeConfigurator.Build();

            builder.ConnectConsumePipe(pipe);
        }
    }
}