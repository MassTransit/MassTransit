// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.SubscriptionConfigurators
{
    using System.Collections.Generic;
    using Configurators;
    using EndpointConfigurators;
    using PipeConfigurators;
    using Pipeline;


    public class HandlerConfigurator<TMessage> :
        IHandlerConfigurator<TMessage>,
        IReceiveEndpointBuilderConfigurator
        where TMessage : class
    {
        readonly HandlerPipeBuilderConfigurator<TMessage> _handlerConfigurator;
        readonly PipeConfigurator<ConsumeContext<TMessage>> _pipeConfigurator;

        public HandlerConfigurator(MessageHandler<TMessage> handler)
        {
            _pipeConfigurator = new PipeConfigurator<ConsumeContext<TMessage>>();
            _handlerConfigurator = new HandlerPipeBuilderConfigurator<TMessage>(handler);
        }

        public void AddPipeBuilderConfigurator(IPipeBuilderConfigurator<ConsumeContext<TMessage>> configurator)
        {
            _pipeConfigurator.AddPipeBuilderConfigurator(configurator);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            _pipeConfigurator.AddPipeBuilderConfigurator(_handlerConfigurator);

            IPipe<ConsumeContext<TMessage>> pipe = _pipeConfigurator.Build();

            builder.Connect(pipe);
        }
    }
}