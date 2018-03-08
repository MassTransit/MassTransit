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
namespace MassTransit.ConsumeConfigurators
{
    using System;


    public interface IBatchConfigurator<out TMessage> :
        IConsumeConfigurator
        where TMessage : class
    {
        /// <summary>
        /// Set the maximum time to wait for messages before the batch is automatically completed
        /// </summary>
        TimeSpan TimeLimit { set; }

        /// <summary>
        /// Set the maximum number of messages which can be added to a single batch
        /// </summary>
        int MessageLimit { set; }

        /// <summary>
        /// Specify the consumer factory for the batch message consumer
        /// </summary>
        /// <param name="consumerFactory"></param>
        /// <typeparam name="TConsumer"></typeparam>
        void Consumer<TConsumer>(IConsumerFactory<TConsumer> consumerFactory)
            where TConsumer : class, IConsumer<Batch<TMessage>>;
    }
}