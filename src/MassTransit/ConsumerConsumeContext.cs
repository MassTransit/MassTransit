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
namespace MassTransit
{
    /// <summary>
    /// A consumer and consume context mixed together, carrying both a consumer and the message
    /// consume context.
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface ConsumerConsumeContext<out TConsumer, out TMessage> :
        ConsumerConsumeContext<TConsumer>,
        ConsumeContext<TMessage>
        where TMessage : class
        where TConsumer : class
    {
        /// <summary>
        /// The consumer which will handle the message
        /// </summary>
        TConsumer Consumer { get; }

        /// <summary>
        /// Return the original consume context without the consumer
        /// </summary>
        ConsumeContext<TMessage> ConsumeContext { get; }
    }


    public interface ConsumerConsumeContext<out TConsumer> :
        ConsumeContext
        where TConsumer : class
    {
        /// <summary>
        /// Return the original consumer/message combined context, reapplying the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ConsumerConsumeContext<TConsumer, T> PopContext<T>()
            where T : class;
    }
}