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

        public InboundPipe()
        {
            _filter = new MessageTypeConsumeFilter();
        }

        public Task Send(ConsumeContext context)
        {
            return _filter.Send(context, Cache.EndPipe);
        }

        public ConnectHandle Connect<T>(IConsumeFilter<T> filter)
            where T : class
        {
            return _filter.Connect(filter);
        }

        public ConnectHandle Connect<TMessage>(IMessageInterceptor<TMessage> interceptor)
            where TMessage : class
        {
            return _filter.Connect(interceptor);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return _filter.Inspect(inspector);
        }


        static class Cache
        {
            internal static readonly IConsumePipe EndPipe = new End();
        }


        class End :
            IConsumePipe
        {
            async Task IPipe<ConsumeContext>.Send(ConsumeContext context)
            {
            }

            public bool Inspect(IPipeInspector inspector)
            {
                return true;
            }
        }
    }
}