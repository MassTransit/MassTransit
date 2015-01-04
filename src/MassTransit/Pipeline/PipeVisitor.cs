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
namespace MassTransit.Pipeline
{
    using System.Linq;
    using Filters;
    using Internals.Extensions;


    public class PipeVisitor :
        IConsumerFilterVisitor,
        IConsumeMessageFilterVisitor
    {
        bool IConsumerFilterVisitor.Visit<TConsumer>(IFilter<ConsumerConsumeContext<TConsumer>> filter, FilterVisitorCallback callback)
        {
            return VisitConsumerConsumeFilter(filter, callback);
        }

        bool IConsumerFilterVisitor.Visit<TConsumer, TMessage>(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> filter,
            FilterVisitorCallback callback)
        {
            return VisitConsumerConsumeFilter(filter, callback);
        }

        bool IPipeVisitor.Visit<T>(IFilter<T> filter)
        {
            return VisitFilter(filter, _ => true);
        }

        bool IPipeVisitor.Visit<T>(IFilter<T> filter, FilterVisitorCallback callback)
        {
            return VisitFilter(filter, callback);
        }

        bool IPipeVisitor.Visit<T>(IPipe<T> pipe)
        {
            return VisitPipe(pipe, _ => true);
        }

        bool IPipeVisitor.Visit<T>(IPipe<T> pipe, PipeVisitorCallback callback)
        {
            return VisitPipe(pipe, callback);
        }

        protected virtual bool VisitFilter<T>(IFilter<T> filter, FilterVisitorCallback callback)
            where T : class, PipeContext
        {
            var consumeFilter = filter as MessageTypeConsumeFilter;
            if (consumeFilter != null)
                return VisitMessageTypeConsumeFilter(consumeFilter, callback);

            if (typeof(T).HasInterface(typeof(ConsumerConsumeContext<,>)))
            {
                var types = typeof(T).GetClosingArguments(typeof(ConsumerConsumeContext<,>)).ToArray();

                return ConsumerFilterVisitorAdapterCache.GetAdapter(types).Visit(this, filter, callback);
            }

            if (typeof(T).HasInterface(typeof(ConsumeContext<>)))
            {
                var types = typeof(T).GetClosingArguments(typeof(ConsumeContext<>)).ToArray();

                return ConsumeMessageFilterVisitorAdapterCache.GetAdapter(types).Visit(this, filter, callback);
            }

            return callback(this);
        }

        bool IConsumeMessageFilterVisitor.Visit<T>(IFilter<ConsumeContext<T>> filter, FilterVisitorCallback callback)
        {
            if (filter is TeeConsumeFilter<T>)
                return VisitTeeConsumeFilter((TeeConsumeFilter<T>)filter, callback);
            if (filter is HandlerMessageFilter<T>)
                return VisitHandlerMessageFilter((HandlerMessageFilter<T>)filter, callback);
            if (filter is RetryFilter<ConsumeContext<T>>)
                return VisitRetryConsumeFilter((RetryFilter<ConsumeContext<T>>)filter, callback);

            return VisitUnknownFilter(filter, callback);
        }

        protected virtual bool VisitPipe<T>(IPipe<T> pipe, PipeVisitorCallback callback)
            where T : class, PipeContext
        {
            return callback(this);
        }

        protected virtual bool VisitUnknownFilter<T>(IFilter<T> filter, FilterVisitorCallback callback)
            where T : class, PipeContext
        {
            return callback(this);
        }

        protected virtual bool VisitMessageTypeConsumeFilter(MessageTypeConsumeFilter filter, FilterVisitorCallback callback)
        {
            return callback(this);
        }

        protected virtual bool VisitTeeConsumeFilter<T>(TeeConsumeFilter<T> filter, FilterVisitorCallback callback)
            where T : class
        {
            return callback(this);
        }

        protected virtual bool VisitHandlerMessageFilter<T>(HandlerMessageFilter<T> filter, FilterVisitorCallback callback)
            where T : class
        {
            return callback(this);
        }

        protected virtual bool VisitRetryConsumeFilter<T>(RetryFilter<ConsumeContext<T>> filter, FilterVisitorCallback callback)
            where T : class
        {
            return callback(this);
        }

        protected virtual bool VisitMethodConsumerMessageFilter<TConsumer, T>(MethodConsumerMessageFilter<TConsumer, T> filter,
            FilterVisitorCallback callback)
            where TConsumer : class, IConsumer<T>
            where T : class
        {
            return callback(this);
        }

        protected virtual bool VisitConsumerSplitFilter<TConsumer, T>(ConsumerSplitFilter<TConsumer, T> filter,
            FilterVisitorCallback callback)
            where TConsumer : class
            where T : class
        {
            return callback(this);
        }

        protected virtual bool VisitConsumerConsumeFilter<TConsumer>(IFilter<ConsumerConsumeContext<TConsumer>> filter,
            FilterVisitorCallback callback)
            where TConsumer : class, IConsumer
        {
            return callback(this);
        }

        protected virtual bool VisitConsumerConsumeFilter<TConsumer, T>(IFilter<ConsumerConsumeContext<TConsumer, T>> filter,
            FilterVisitorCallback callback)
            where TConsumer : class, IConsumer<T>
            where T : class
        {
            if (filter is ConsumerSplitFilter<TConsumer, T>)
                return VisitConsumerSplitFilter((ConsumerSplitFilter<TConsumer, T>)filter, callback);
            if (filter is MethodConsumerMessageFilter<TConsumer, T>)
                return VisitMethodConsumerMessageFilter((MethodConsumerMessageFilter<TConsumer, T>)filter, callback);
            return callback(this);
        }
    }
}