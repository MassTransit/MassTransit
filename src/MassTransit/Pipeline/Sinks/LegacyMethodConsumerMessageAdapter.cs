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
namespace MassTransit.Pipeline.Sinks
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Util;


    /// <summary>
    /// Dispatches the ConsumeContext to the consumer method for the specified message type
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class LegacyMethodConsumerMessageAdapter<TConsumer, TMessage> :
        IConsumerMessageAdapter<TConsumer, TMessage>
        where TConsumer : class, IMessageConsumer<TMessage>
        where TMessage : class
    {
        public async Task Send(ConsumeContext<Tuple<TConsumer, ConsumeContext<TMessage>>> context,
            IPipe<ConsumeContext<Tuple<TConsumer, ConsumeContext<TMessage>>>> next)
        {
            var messageConsumer = context.Message.Item1 as IMessageConsumer<TMessage>;
            if (messageConsumer == null)
            {
                string message = string.Format("Consumer type {0} is not a consumer of message type {1}",
                    TypeMetadataCache<TConsumer>.ShortName, TypeMetadataCache<TMessage>.ShortName);

                throw new ConsumerMessageException(message);
            }

            IConsumeContext<TMessage> consumeContext = new ConsumeContextAdapter<TMessage>(context.Message.Item2);

            using (consumeContext.CreateScope())
            {
                messageConsumer.Consume(consumeContext.Message);
            }
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this);
        }
    }
}