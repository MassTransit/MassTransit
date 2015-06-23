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
namespace MassTransit.Pipeline.Pipes
{
    using System;
    using System.Threading.Tasks;
    using Filters;
    using Monitoring.Introspection;


    public class ConsumePipe :
        IConsumePipe
    {
        readonly MessageTypeConsumeFilter _filter;
        readonly IPipe<ConsumeContext> _pipe;

        public ConsumePipe(MessageTypeConsumeFilter messageTypeConsumeFilter, IPipe<ConsumeContext> pipe)
        {
            if (messageTypeConsumeFilter == null)
                throw new ArgumentNullException("messageTypeConsumeFilter");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            _filter = messageTypeConsumeFilter;
            _pipe = pipe;
        }

        Task IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("consumePipe");

            return _pipe.Probe(scope);
        }

        Task IPipe<ConsumeContext>.Send(ConsumeContext context)
        {
            return _pipe.Send(context);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<TMessage>(IConsumeMessageObserver<TMessage> observer)
        {
            return _filter.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            return _filter.ConnectConsumePipe(pipe);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            return _filter.ConnectRequestPipe(requestId, pipe);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _filter.ConnectConsumeObserver(observer);
        }
    }
}