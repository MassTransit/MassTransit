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
namespace MassTransit.ConsumeConnectors
{
    using System;
    using GreenPipes;
    using GreenPipes.Builders;
    using PipeConfigurators;
    using Pipeline;


    /// <summary>
    /// Connects a message handler to a pipe
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class HandlerConnector<TMessage> :
        IHandlerConnector<TMessage>
        where TMessage : class
    {
        public ConnectHandle ConnectHandler(IConsumePipeConnector consumePipe, MessageHandler<TMessage> handler,
            params IPipeSpecification<ConsumeContext<TMessage>>[] pipeSpecifications)
        {
            IPipe<ConsumeContext<TMessage>> pipe = Pipe.New<ConsumeContext<TMessage>>(x =>
            {
                foreach (var specification in pipeSpecifications)
                    x.AddPipeSpecification(specification);

                x.AddPipeSpecification(new HandlerPipeSpecification<TMessage>(handler));
            });

            return consumePipe.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectRequestHandler(IRequestPipeConnector consumePipe, Guid requestId, MessageHandler<TMessage> handler,
            IBuildPipeConfigurator<ConsumeContext<TMessage>> configurator)
        {
            configurator.AddPipeSpecification(new HandlerPipeSpecification<TMessage>(handler));

            return consumePipe.ConnectRequestPipe(requestId, configurator.Build());
        }
    }
}
