// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Scoping.ConsumerContexts
{
    using System;


    public class CreatedConsumerScopeContext<TScope> :
        IConsumerScopeContext
        where TScope : IDisposable
    {
        readonly TScope _scope;

        public CreatedConsumerScopeContext(TScope scope, ConsumeContext context)
        {
            _scope = scope;
            Context = context;
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }

        public ConsumeContext Context { get; }
    }


    public class CreatedConsumerScopeContext<TScope, TConsumer, T> :
        IConsumerScopeContext<TConsumer, T>
        where TScope : IDisposable
        where TConsumer : class
        where T : class
    {
        readonly TScope _scope;
        readonly Action<TConsumer> _disposeCallback;

        public CreatedConsumerScopeContext(TScope scope, ConsumerConsumeContext<TConsumer, T> context, Action<TConsumer> disposeCallback = null)
        {
            _scope = scope;
            _disposeCallback = disposeCallback;
            Context = context;
        }

        public void Dispose()
        {
            _disposeCallback?.Invoke(Context.Consumer);
            _scope?.Dispose();
        }

        public ConsumerConsumeContext<TConsumer, T> Context { get; }
    }
}