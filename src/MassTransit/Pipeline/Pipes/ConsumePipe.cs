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
namespace MassTransit.Pipeline.Pipes
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Filters;


    public class ConsumePipe :
        IConsumePipe
    {
        readonly IDynamicFilter<ConsumeContext, Guid> _dynamicFilter;
        readonly IPipe<ConsumeContext> _pipe;

        public ConsumePipe(IDynamicFilter<ConsumeContext, Guid> dynamicFilter, IPipe<ConsumeContext> pipe)
        {
            _dynamicFilter = dynamicFilter ?? throw new ArgumentNullException(nameof(dynamicFilter));
            _pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("consumePipe");

            _pipe.Probe(scope);
        }

        Task IPipe<ConsumeContext>.Send(ConsumeContext context)
        {
            return _pipe.Send(context);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<TMessage>(IConsumeMessageObserver<TMessage> observer)
        {
            return _dynamicFilter.ConnectObserver(new ConsumeObserverAdapter<TMessage>(observer));
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            return _dynamicFilter.ConnectPipe(pipe);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            return _dynamicFilter.ConnectPipe(requestId, pipe);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _dynamicFilter.ConnectObserver(new ConsumeObserverAdapter(observer));
        }
    }
}