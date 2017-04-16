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
namespace MassTransit.ConsumeConfigurators
{
    using System;
    using GreenPipes;


    public interface IConsumerMessageConfigurator<TMessage> :
        IPipeConfigurator<ConsumeContext<TMessage>>
        where TMessage : class
    {
    }


    public interface IConsumerMessageConfigurator<TConsumer, TMessage> :
        IPipeConfigurator<ConsumerConsumeContext<TConsumer, TMessage>>
        where TConsumer : class
        where TMessage : class
    {
        /// <summary>
        /// Add middleware to the consumer pipeline, for the specified message type, which is
        /// invoked after the consumer factory.
        /// </summary>
        /// <param name="configure">The callback to configure the message pipeline</param>
        void Message(Action<IConsumerMessageConfigurator<TMessage>> configure);
    }
}