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
    using Sinks;


    public class PipeInspectorBase :
        IPipeInspector
    {
        public virtual bool Inspect<T>(IFilter<T> filter)
            where T : class, PipeContext
        {
            return Inspect(filter, (_, __) => true);
        }

        public virtual bool Inspect<T>(IFilter<T> filter, FilterInspectorCallback<T> callback)
            where T : class, PipeContext
        {
            return callback(this, filter);
        }

        public bool Inspect<T>(IConsumeFilter<T> filter)
            where T : class
        {
            return Inspect(filter, (_, __) => true);
        }

        public bool Inspect<T>(IConsumeFilter<T> filter, FilterInspectorCallback<ConsumeContext<T>> callback)
            where T : class
        {
            if (filter is TeeConsumeFilter<T>)
                return Inspect((TeeConsumeFilter<T>)filter, (x, _) => callback(x, filter));
            if (filter is HandlerMessageFilter<T>)
                return Inspect((HandlerMessageFilter<T>)filter, (x, _) => callback(x, filter));

            return callback(this, filter);
        }

        public virtual bool Inspect<T>(IPipe<T> pipe)
            where T : class, PipeContext
        {
            return Inspect(pipe, (_, __) => true);
        }

        public virtual bool Inspect<T>(IPipe<T> pipe, PipeInspectorCallback<T> callback)
            where T : class, PipeContext
        {
            return callback(this, pipe);
        }

        public bool Inspect<T>(IConsumePipe<T> pipe)
            where T : class
        {
            return Inspect(pipe, (_, __) => true);
        }

        public bool Inspect<T>(IConsumePipe<T> pipe, PipeInspectorCallback<ConsumeContext<T>> callback)
            where T : class
        {
            return callback(this, pipe);
        }

        protected virtual bool Inspect<T>(TeeConsumeFilter<T> filter,
            FilterInspectorCallback<ConsumeContext<T>> callback)
            where T : class
        {
            return callback(this, filter);
        }

        protected virtual bool Inspect<T>(HandlerMessageFilter<T> filter,
            FilterInspectorCallback<ConsumeContext<T>> callback)
            where T : class
        {
            return callback(this, filter);
        }
    }
}