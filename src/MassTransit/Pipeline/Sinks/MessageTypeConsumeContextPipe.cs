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
    using Util;


    public class MessageTypeConsumeContextPipe :
        IConsumeContextPipe,
        IConnectPipe,
        IConnectMessageInterceptor
    {
        readonly ConcurrentDictionary<Type, IConsumeContextPipe> _pipes;

        public MessageTypeConsumeContextPipe()
        {
            _pipes = new ConcurrentDictionary<Type, IConsumeContextPipe>();
        }

        public ConnectHandle Connect<T>([NotNull] IMessageInterceptor<T> interceptor)
            where T : class
        {
            if (interceptor == null)
                throw new ArgumentNullException("interceptor");

            IConnectMessageInterceptor messagePipe = GetPipe<T, IConnectMessageInterceptor>();

            ConnectHandle handle = messagePipe.Connect(interceptor);

            return handle;
        }

        public ConnectHandle Connect<T>(IConsumeContextPipe<T> pipe)
            where T : class
        {
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            IConnectPipe messagePipe = GetPipe<T, IConnectPipe>();

            ConnectHandle handle = messagePipe.Connect(pipe);

            return handle;
        }

        public async Task Send(ConsumeContext context)
        {
            foreach (IConsumeContextPipe pipe in _pipes.Values)
                await pipe.Send(context);
        }

        public bool Inspect(IConsumeContextPipeInspector inspector)
        {
            return inspector.Inspect(this, (x, _) => _pipes.Values.All(pipe => pipe.Inspect(x)));
        }

        TResult GetPipe<T, TResult>()
            where T : class
            where TResult : class
        {
            return (TResult)_pipes.GetOrAdd(typeof(T), x => (IConsumeContextPipe)new MessageConsumeContextPipe<T>());
        }
    }
}