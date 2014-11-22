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
namespace MassTransit.SubscriptionConnectors
{
    using System;
    using PipeConfigurators;
    using Pipeline;


    public class HandlerConnectorImpl<T> :
        HandlerConnector<T>
        where T : class
    {
        public ConnectHandle Connect(IInboundPipe inboundPipe, MessageHandler<T> handler,
            params IFilter<ConsumeContext<T>>[] filters)
        {
            IPipe<ConsumeContext<T>> pipe = Pipe.New<ConsumeContext<T>>(x =>
            {
                foreach (var filter in filters)
                    x.Filter(filter);

                x.AddPipeBuilderConfigurator(new HandlerPipeBuilderConfigurator<T>(handler));
            });

            return inboundPipe.Connect(pipe);
        }

        public ConnectHandle Connect(IInboundPipe inboundPipe, Guid requestId, MessageHandler<T> handler,
            params IFilter<ConsumeContext<T>>[] filters)
        {
            IPipe<ConsumeContext<T>> pipe = Pipe.New<ConsumeContext<T>>(x =>
            {
                foreach (var filter in filters)
                    x.Filter(filter);

                x.AddPipeBuilderConfigurator(new HandlerPipeBuilderConfigurator<T>(handler));
            });

            return inboundPipe.Connect(requestId, pipe);
        }
    }
}