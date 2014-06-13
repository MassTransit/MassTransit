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
namespace MassTransit.Pipeline.Sinks
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;


    public class MessageTypeConsumeFilter :
        IConsumeFilter,
        IConsumeFilterConnector,
        IConnectMessageInterceptor
    {
        readonly ConcurrentDictionary<Type, IConsumeFilter> _pipes;

        public MessageTypeConsumeFilter()
        {
            _pipes = new ConcurrentDictionary<Type, IConsumeFilter>();
        }

        public ConnectHandle Connect<T>(IMessageInterceptor<T> interceptor)
            where T : class
        {
            if (interceptor == null)
                throw new ArgumentNullException("interceptor");

            IConnectMessageInterceptor messagePipe = GetPipe<T, IConnectMessageInterceptor>();

            ConnectHandle handle = messagePipe.Connect(interceptor);

            return handle;
        }

        public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            foreach (IConsumeFilter pipe in _pipes.Values)
                await pipe.Send(context, next);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, (x, _) => _pipes.Values.All(pipe => pipe.Inspect(x)));
        }

        public ConnectHandle Connect<T>(IConsumeFilter<T> filter)
            where T : class
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            IConsumeFilterConnector messagePipe = GetPipe<T, IConsumeFilterConnector>();

            ConnectHandle handle = messagePipe.Connect(filter);

            return handle;
        }

        TResult GetPipe<T, TResult>()
            where T : class
            where TResult : class
        {
            return (TResult)_pipes.GetOrAdd(typeof(T), x => (IConsumeFilter)new MessageConsumeFilter<T>());
        }
    }
}