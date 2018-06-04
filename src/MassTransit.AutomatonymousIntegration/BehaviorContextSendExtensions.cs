// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public static partial class BehaviorContextExtensions
    {
        public static Task Send<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message)
            where T : class
        {
            return GetConsumeContext(context).Send(message);
        }

        public static Task Send<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Send(message, sendPipe);
        }

        public static Task Send<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message, IPipe<SendContext> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Send(message, sendPipe);
        }

        public static Task Send<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message)
        {
            return GetConsumeContext(context).Send(message);
        }

        public static Task Send<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message, Type messageType)
        {
            return GetConsumeContext(context).Send(message, messageType);
        }

        public static Task Send<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message, IPipe<SendContext> sendPipe)
        {
            return GetConsumeContext(context).Send(message, sendPipe);
        }

        public static Task Send<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message, Type messageType,
            IPipe<SendContext> sendPipe)
        {
            return GetConsumeContext(context).Send(message, messageType, sendPipe);
        }

        public static Task Send<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, object values)
            where T : class
        {
            return GetConsumeContext(context).Send<T>(values);
        }

        public static Task Send<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, object values, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Publish(values, sendPipe);
        }

        public static Task Send<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, object values, IPipe<SendContext> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Send<T>(values, sendPipe);
        }
    }
}