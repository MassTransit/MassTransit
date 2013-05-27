// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;
    using Context;

    public static class ContextExtensions
    {
        public static void ForwardUsingOriginalContext(this ISendContext target,
            IConsumeContext source)
        {
            target.SetRequestId(source.RequestId);
            target.SetConversationId(source.ConversationId);
            target.SetCorrelationId(source.CorrelationId);
            target.SetSourceAddress(source.SourceAddress);
            target.SetResponseAddress(source.ResponseAddress);
            target.SetFaultAddress(source.FaultAddress);
            target.SetNetwork(source.Network);
            if (source.ExpirationTime.HasValue)
                target.SetExpirationTime(source.ExpirationTime.Value);
            target.SetRetryCount(source.RetryCount);

            foreach (var header in source.Headers)
                target.SetHeader(header.Key, header.Value);

            var inputAddress = source.InputAddress != null
                                   ? source.InputAddress.ToString()
                                   : source.DestinationAddress != null
                                         ? source.DestinationAddress.ToString()
                                         : null;

            if(!string.IsNullOrEmpty(inputAddress))
                target.SetHeader("mt.forwarder.uri", source.DestinationAddress.ToString());
        }

        public static IConsumeContext Context(this IConsumer instance)
        {
            return ContextStorage.Context();
        }

        public static IConsumeContext Context(this IServiceBus bus)
        {
            return ContextStorage.Context();
        }

        public static IConsumeContext<T> MessageContext<T>(this Consumes<T>.All instance)
            where T : class
        {
            return ContextStorage.MessageContext<T>();
        }

        public static IConsumeContext<T> MessageContext<T>(this IServiceBus bus)
            where T : class
        {
            return ContextStorage.MessageContext<T>();
        }

        public static void Context(this IServiceBus bus, Action<IConsumeContext> contextCallback)
        {
            ContextStorage.Context(contextCallback);
        }

        public static TResult Context<TResult>(this IServiceBus bus, Func<IConsumeContext, TResult> contextCallback)
        {
            return ContextStorage.Context(contextCallback);
        }
    }
}