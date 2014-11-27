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
    using System.Linq;
    using System.Reflection;
    using Filters;


    public class PipeInspectorBase :
        IPipeInspector
    {
        public virtual bool Inspect<T>(IFilter<T> filter)
            where T : class, PipeContext
        {
            return Inspect(filter, _ => true);
        }

        public virtual bool Inspect<T>(IFilter<T> filter, FilterInspectorCallback callback)
            where T : class, PipeContext
        {
            if (filter is MessageTypeConsumeFilter)
                return Inspect((MessageTypeConsumeFilter)filter, callback);

            return callback(this);
        }

        public bool Inspect<T>(IFilter<ConsumeContext<T>> filter)
            where T : class
        {
            return Inspect(filter, _ => true);
        }

        public bool Inspect<T>(IFilter<ConsumeContext<T>> filter, FilterInspectorCallback callback)
            where T : class
        {
            if (filter is TeeConsumeFilter<T>)
                return Inspect((TeeConsumeFilter<T>)filter, callback);
            if (filter is HandlerMessageFilter<T>)
                return Inspect((HandlerMessageFilter<T>)filter, callback);

            if (filter.GetType().IsGenericType && filter.GetType().GetGenericTypeDefinition() == typeof(ConsumerMessageFilter<,>))
            {
                var method = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(x => x.Name == "Inspect")
                    .Where(
                        x =>
                            x.IsGenericMethod
                                && x.GetParameters().First().ParameterType.GetGenericTypeDefinition() == typeof(ConsumerMessageFilter<,>))
                    .First();
                var genericMethod = method.MakeGenericMethod(filter.GetType().GetGenericArguments());

                return (bool)genericMethod.Invoke(this, new object[] {filter, callback});
            }

            return Unknown(filter, callback);
        }

        protected virtual bool Unknown<T>(IFilter<T> filter, FilterInspectorCallback callback) 
            where T : class, PipeContext
        {
            return callback(this);
        }

        public virtual bool Inspect<T>(IPipe<T> pipe)
            where T : class, PipeContext
        {
            return Inspect(pipe, _ => true);
        }

        public virtual bool Inspect<T>(IPipe<T> pipe, PipeInspectorCallback callback)
            where T : class, PipeContext
        {
            return callback(this);
        }

        protected virtual bool Inspect(MessageTypeConsumeFilter filter, FilterInspectorCallback callback)
        {
            return callback(this);
        }

        protected virtual bool Inspect<T>(TeeConsumeFilter<T> filter, FilterInspectorCallback callback)
            where T : class
        {
            return callback(this);
        }

        protected virtual bool Inspect<T>(HandlerMessageFilter<T> filter, FilterInspectorCallback callback)
            where T : class
        {
            return callback(this);
        }

        protected virtual bool Inspect<TConsumer, T>(ConsumerMessageFilter<TConsumer, T> filter, FilterInspectorCallback callback)
            where TConsumer : class
            where T : class
        {
            return callback(this);
        }
    }
}