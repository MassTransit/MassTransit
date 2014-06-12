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
    using SubscriptionConnectors;


    public static class InboundMessagePipeExtensions
    {
        public static ConnectHandle ConnectHandler<T>(this IInboundMessagePipe pipe, MessageHandler<T> handler)
            where T : class
        {
            return HandlerMetadataCache<T>.Connector.Connect(pipe, handler);
        }

        public static ConnectHandle ConnectConsumer<T>(this IInboundMessagePipe pipe, IAsyncConsumerFactory<T> consumerFactory)
            where T : class
        {
            return ConsumerMetadataCache<T>.Connector.Connect(pipe, consumerFactory);
        }
    }
}