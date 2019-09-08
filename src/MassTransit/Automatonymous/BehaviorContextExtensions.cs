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
namespace Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public static partial class BehaviorContextExtensions
    {
        public static Task Publish<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message)
            where T : class
        {
            return GetConsumeContext(context).Publish(message);
        }

        public static Task Publish<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message, IPipe<PublishContext<T>> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Publish(message, sendPipe);
        }

        public static Task Publish<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message, IPipe<PublishContext> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Publish(message, sendPipe);
        }

        public static Task Publish<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message)
        {
            return GetConsumeContext(context).Publish(message);
        }

        public static Task Publish<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message, Type messageType)
        {
            return GetConsumeContext(context).Publish(message, messageType);
        }

        public static Task Publish<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message, IPipe<PublishContext> sendPipe)
        {
            return GetConsumeContext(context).Publish(message, sendPipe);
        }

        public static Task Publish<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message, Type messageType,
            IPipe<PublishContext> sendPipe)
        {
            return GetConsumeContext(context).Publish(message, messageType, sendPipe);
        }

        public static Task Publish<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, object values)
            where T : class
        {
            return GetConsumeContext(context).Publish<T>(values);
        }

        public static Task Publish<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, object values, IPipe<PublishContext<T>> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Publish(values, sendPipe);
        }

        public static Task Publish<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, object values, IPipe<PublishContext> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Publish<T>(values, sendPipe);
        }

        public static Task<ISendEndpoint> GetSendEndpoint<TInstance, TData>(this BehaviorContext<TInstance, TData> context, Uri address)
        {
            return GetConsumeContext(context).GetSendEndpoint(address);
        }

        public static Task RespondAsync<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message)
            where T : class
        {
            return GetConsumeContext(context).RespondAsync(message);
        }

        public static Task RespondAsync<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).RespondAsync(message, sendPipe);
        }

        public static Task RespondAsync<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message, IPipe<SendContext> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).RespondAsync(message, sendPipe);
        }

        public static Task RespondAsync<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message)
        {
            return GetConsumeContext(context).RespondAsync(message);
        }

        public static Task RespondAsync<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message, Type messageType)
        {
            return GetConsumeContext(context).RespondAsync(message, messageType);
        }

        public static Task RespondAsync<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message, IPipe<SendContext> sendPipe)
        {
            return GetConsumeContext(context).RespondAsync(message, sendPipe);
        }

        public static Task RespondAsync<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message, Type messageType,
            IPipe<SendContext> sendPipe)
        {
            return GetConsumeContext(context).RespondAsync(message, messageType, sendPipe);
        }

        public static Task RespondAsync<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, object values)
            where T : class
        {
            return GetConsumeContext(context).RespondAsync<T>(values);
        }

        public static Task RespondAsync<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, object values, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).RespondAsync(values, sendPipe);
        }

        public static Task RespondAsync<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, object values, IPipe<SendContext> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).RespondAsync<T>(values, sendPipe);
        }

        public static void Respond<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message)
            where T : class
        {
            GetConsumeContext(context).Respond(message);
        }

        static ConsumeContext GetConsumeContext<TInstance>(BehaviorContext<TInstance> context)
        {
            if (context.TryGetPayload(out ConsumeContext consumeContext))
                return consumeContext;

            throw new ArgumentException("The ConsumeContext was not present", nameof(context));
        }

        public static Task Publish<TInstance, T>(this BehaviorContext<TInstance> context, T message)
            where T : class
        {
            return GetConsumeContext(context).Publish(message);
        }

        public static Task Publish<TInstance, T>(this BehaviorContext<TInstance> context, T message, IPipe<PublishContext<T>> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Publish(message, sendPipe);
        }

        public static Task Publish<TInstance, T>(this BehaviorContext<TInstance> context, T message, IPipe<PublishContext> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Publish(message, sendPipe);
        }

        public static Task Publish<TInstance>(this BehaviorContext<TInstance> context, object message)
        {
            return GetConsumeContext(context).Publish(message);
        }

        public static Task Publish<TInstance>(this BehaviorContext<TInstance> context, object message, Type messageType)
        {
            return GetConsumeContext(context).Publish(message, messageType);
        }

        public static Task Publish<TInstance>(this BehaviorContext<TInstance> context, object message, IPipe<PublishContext> sendPipe)
        {
            return GetConsumeContext(context).Publish(message, sendPipe);
        }

        public static Task Publish<TInstance>(this BehaviorContext<TInstance> context, object message, Type messageType,
            IPipe<PublishContext> sendPipe)
        {
            return GetConsumeContext(context).Publish(message, messageType, sendPipe);
        }

        public static Task Publish<TInstance, T>(this BehaviorContext<TInstance> context, object values)
            where T : class
        {
            return GetConsumeContext(context).Publish<T>(values);
        }

        public static Task Publish<TInstance, T>(this BehaviorContext<TInstance> context, object values, IPipe<PublishContext<T>> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Publish(values, sendPipe);
        }

        public static Task Publish<TInstance, T>(this BehaviorContext<TInstance> context, object values, IPipe<PublishContext> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Publish<T>(values, sendPipe);
        }

        public static Task<ISendEndpoint> GetSendEndpoint<TInstance>(this BehaviorContext<TInstance> context, Uri address)
        {
            return GetConsumeContext(context).GetSendEndpoint(address);
        }

        public static Task RespondAsync<TInstance, T>(this BehaviorContext<TInstance> context, T message)
            where T : class
        {
            return GetConsumeContext(context).RespondAsync(message);
        }

        public static Task RespondAsync<TInstance, T>(this BehaviorContext<TInstance> context, T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).RespondAsync(message, sendPipe);
        }

        public static Task RespondAsync<TInstance, T>(this BehaviorContext<TInstance> context, T message, IPipe<SendContext> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).RespondAsync(message, sendPipe);
        }

        public static Task RespondAsync<TInstance>(this BehaviorContext<TInstance> context, object message)
        {
            return GetConsumeContext(context).RespondAsync(message);
        }

        public static Task RespondAsync<TInstance>(this BehaviorContext<TInstance> context, object message, Type messageType)
        {
            return GetConsumeContext(context).RespondAsync(message, messageType);
        }

        public static Task RespondAsync<TInstance>(this BehaviorContext<TInstance> context, object message, IPipe<SendContext> sendPipe)
        {
            return GetConsumeContext(context).RespondAsync(message, sendPipe);
        }

        public static Task RespondAsync<TInstance>(this BehaviorContext<TInstance> context, object message, Type messageType,
            IPipe<SendContext> sendPipe)
        {
            return GetConsumeContext(context).RespondAsync(message, messageType, sendPipe);
        }

        public static Task RespondAsync<TInstance, T>(this BehaviorContext<TInstance> context, object values)
            where T : class
        {
            return GetConsumeContext(context).RespondAsync<T>(values);
        }

        public static Task RespondAsync<TInstance, T>(this BehaviorContext<TInstance> context, object values, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).RespondAsync(values, sendPipe);
        }

        public static Task RespondAsync<TInstance, T>(this BehaviorContext<TInstance> context, object values, IPipe<SendContext> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).RespondAsync<T>(values, sendPipe);
        }

        public static void Respond<TInstance, T>(this BehaviorContext<TInstance> context, T message)
            where T : class
        {
            GetConsumeContext(context).Respond(message);
        }
    }
}