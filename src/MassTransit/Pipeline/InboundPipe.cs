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
namespace MassTransit.Pipeline
{
    using System.Threading.Tasks;
    using Sinks;


    public class InboundPipe :
        IInboundPipe
    {
        readonly MessageTypeConsumeFilter _filter;
        readonly IPipe<ConsumeContext> _pipe;

        public InboundPipe()
        {
            _filter = new MessageTypeConsumeFilter();

            _pipe = Pipe.New<ConsumeContext>(x => x.Filter(_filter));
        }

        Task IPipe<ConsumeContext>.Send(ConsumeContext context)
        {
            return _pipe.Send(context);
        }

        ConnectHandle IConsumeObserverConnector.Connect<TMessage>(IConsumeObserver<TMessage> observer)
        {
            return _filter.Connect(observer);
        }

        bool IPipe<ConsumeContext>.Inspect(IPipeInspector inspector)
        {
            return _pipe.Inspect(inspector);
        }

        ConnectHandle IConsumeFilterConnector.Connect<T>(IPipe<ConsumeContext<T>> pipe)
        {
            return _filter.Connect(pipe);
        }
    }
}