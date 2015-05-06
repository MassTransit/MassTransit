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
    using PipeConfigurators;
    using Pipeline;


    /// <summary>
    /// Connects a message handler to a pipe
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class MessageObserverConnector<TMessage> :
        ObserverConnector<TMessage>
        where TMessage : class
    {
        public ConnectHandle Connect(IConsumePipeConnector consumePipe, IObserver<ConsumeContext<TMessage>> observer,
            params IFilter<ConsumeContext<TMessage>>[] filters)
        {
            IPipe<ConsumeContext<TMessage>> pipe = Pipe.New<ConsumeContext<TMessage>>(x =>
            {
                foreach (var filter in filters)
                    x.Filter(filter);

                x.AddPipeSpecification(new ObserverPipeSpecification<TMessage>(observer));
            });

            return consumePipe.ConnectConsumePipe(pipe);
        }

        public ConnectHandle Connect(IRequestPipeConnector consumePipe, Guid requestId, IObserver<ConsumeContext<TMessage>> observer,
            params IFilter<ConsumeContext<TMessage>>[] filters)
        {
            IPipe<ConsumeContext<TMessage>> pipe = Pipe.New<ConsumeContext<TMessage>>(x =>
            {
                foreach (var filter in filters)
                    x.Filter(filter);

                x.AddPipeSpecification(new ObserverPipeSpecification<TMessage>(observer));
            });

            return consumePipe.ConnectRequestPipe(requestId, pipe);
        }
    }
}