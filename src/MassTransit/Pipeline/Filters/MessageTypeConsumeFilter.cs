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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;


    public class MessageTypeConsumeFilter :
        IFilter<ConsumeContext>,
        IConsumeFilterConnector,
        IConsumeObserverConnector
    {
        readonly ConcurrentDictionary<Type, IFilter<ConsumeContext>> _pipes;

        public MessageTypeConsumeFilter()
        {
            _pipes = new ConcurrentDictionary<Type, IFilter<ConsumeContext>>();
        }

        public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            foreach (IFilter<ConsumeContext> pipe in _pipes.Values)
                await pipe.Send(context, next);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, x => _pipes.Values.All(pipe => pipe.Inspect(x)));
        }

        public ConnectHandle Connect<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            IConsumeFilterConnector messagePipe = GetPipe<T, IConsumeFilterConnector>();

            return messagePipe.Connect(pipe);
        }

        public ConnectHandle Connect<T>(IConsumeObserver<T> observer)
            where T : class
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            IConsumeObserverConnector messagePipe = GetPipe<T, IConsumeObserverConnector>();

            return messagePipe.Connect(observer);
        }

        TResult GetPipe<T, TResult>()
            where T : class
            where TResult : class
        {
            return (TResult)_pipes.GetOrAdd(typeof(T), x => new MessageConsumeFilter<T>());
        }
    }
}