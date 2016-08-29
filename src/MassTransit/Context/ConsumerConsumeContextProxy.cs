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
namespace MassTransit.Context
{
    using Util;


    /// <summary>
    /// A consumer instance merged with a message consume context
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumerConsumeContextProxy<TConsumer, TMessage> :
        ConsumeContextProxy<TMessage>,
        ConsumerConsumeContext<TConsumer, TMessage>
        where TMessage : class
        where TConsumer : class
    {
        public ConsumerConsumeContextProxy(ConsumeContext<TMessage> context, TConsumer consumer)
            : base(context)
        {
            Consumer = consumer;
        }

        public ConsumerConsumeContextProxy(ConsumeContext<TMessage> context, IPayloadCache payloadCache, TConsumer consumer)
            : base(context, payloadCache)
        {
            Consumer = consumer;
        }

        public ConsumerConsumeContext<TConsumer, T> PopContext<T>()
            where T : class
        {
            var context = this as ConsumerConsumeContext<TConsumer, T>;
            if (context == null)
                throw new ContextException($"The ConsumeContext<{TypeMetadataCache<TMessage>.ShortName}> could not be cast to {TypeMetadataCache<T>.ShortName}");

            return context;
        }

        public TConsumer Consumer { get; }
    }
}