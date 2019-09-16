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
namespace MassTransit.Pipeline.Filters
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using Util;


    /// <summary>
    ///     Dispatches the ConsumeContext to the consumer method for the specified message type
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class LegacyMethodConsumerMessageFilter<TConsumer, TMessage> :
        IConsumerMessageFilter<TConsumer, TMessage>
        where TConsumer : class, IMessageConsumer<TMessage>
        where TMessage : class
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("consume");
            scope.Add("method", $"Consume({TypeMetadataCache<TMessage>.ShortName} message)");
        }

        [DebuggerNonUserCode]
        Task IFilter<ConsumerConsumeContext<TConsumer, TMessage>>.Send(ConsumerConsumeContext<TConsumer, TMessage> context,
            IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
        {
            if (context.Consumer is IMessageConsumer<TMessage> messageConsumer)
            {
                messageConsumer.Consume(context.Message);

                return TaskUtil.Completed;
            }

            var message = $"Consumer type {TypeMetadataCache<TConsumer>.ShortName} is not a consumer of message type {TypeMetadataCache<TMessage>.ShortName}";

            throw new ConsumerMessageException(message);
        }
    }
}