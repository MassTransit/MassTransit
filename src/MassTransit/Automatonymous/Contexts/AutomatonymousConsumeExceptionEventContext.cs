// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous.Contexts
{
    using System;
    using MassTransit;

    public class AutomatonymousConsumeExceptionEventContext<TInstance, TException> :
        AutomatonymousConsumeEventContext<TInstance>,
        ConsumeExceptionEventContext<TInstance, TException>
        where TException : Exception
    {
        public AutomatonymousConsumeExceptionEventContext(BehaviorExceptionContext<TInstance, TException> context, ConsumeContext consumeContext)
            : base(context, consumeContext)
        {
            Exception = context.Exception;
        }

        public TException Exception { get; }
    }

    public class AutomatonymousConsumeExceptionEventContext<TInstance, TData, TException> :
        AutomatonymousConsumeEventContext<TInstance, TData>,
        ConsumeExceptionEventContext<TInstance, TData, TException>
        where TData : class
        where TException : Exception
    {
        public AutomatonymousConsumeExceptionEventContext(BehaviorExceptionContext<TInstance, TData, TException> context, ConsumeContext<TData> consumeContext)
            : base(context, consumeContext)
        {
            Exception = context.Exception;
        }

        public TException Exception { get; }
    }
}
