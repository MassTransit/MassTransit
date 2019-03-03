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
namespace Automatonymous
{
    using System;
    using Contexts;
    using MassTransit;


    public static class ContextExtensions
    {
        public static ConsumeEventContext<TInstance, TData> CreateConsumeContext<TInstance, TData>(
            this BehaviorContext<TInstance, TData> context)
            where TData : class
        {
            ConsumeContext<TData> consumeContext;
            if (!context.TryGetPayload(out consumeContext))
                throw new ArgumentException("The ConsumeContext was not available");

            return new AutomatonymousConsumeEventContext<TInstance, TData>(context, consumeContext);
        }

        public static ConsumeEventContext<TInstance> CreateConsumeContext<TInstance>(
            this BehaviorContext<TInstance> context)
        {
            ConsumeContext consumeContext;
            if (!context.TryGetPayload(out consumeContext))
                throw new ArgumentException("The ConsumeContext was not available");

            return new AutomatonymousConsumeEventContext<TInstance>(context, consumeContext);
        }

        public static bool TryGetExceptionContext<TInstance, TException>(
            this BehaviorContext<TInstance> context, out ConsumeExceptionEventContext<TInstance, TException> exceptionContext)
            where TException : Exception
        {
            var behaviorExceptionContext = context as BehaviorExceptionContext<TInstance, TException>;
            if (behaviorExceptionContext != null)
            {
                ConsumeContext consumeContext;
                if (!context.TryGetPayload(out consumeContext))
                    throw new ContextException("The consume context could not be retrieved.");

                exceptionContext = new AutomatonymousConsumeExceptionEventContext<TInstance, TException>(behaviorExceptionContext, consumeContext);
                return true;
            }

            exceptionContext = null;
            return false;
        }

        public static bool TryGetExceptionContext<TInstance, TData, TException>(
            this BehaviorContext<TInstance, TData> context, out ConsumeExceptionEventContext<TInstance, TData, TException> exceptionContext)
            where TData : class
            where TException : Exception
        {
            var behaviorExceptionContext = context as BehaviorExceptionContext<TInstance, TData, TException>;
            if (behaviorExceptionContext != null)
            {
                ConsumeContext<TData> consumeContext;
                if (!context.TryGetPayload(out consumeContext))
                    throw new ContextException("The consume context could not be retrieved.");

                exceptionContext = new AutomatonymousConsumeExceptionEventContext<TInstance, TData, TException>(behaviorExceptionContext, consumeContext);
                return true;
            }

            exceptionContext = null;
            return false;
        }
    }
}
