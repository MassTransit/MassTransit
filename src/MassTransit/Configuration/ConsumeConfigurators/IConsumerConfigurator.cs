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
namespace MassTransit.ConsumeConfigurators
{
    using System;
    using GreenPipes;


    public interface IConsumerConfigurator :
        IConsumeConfigurator,
        IConsumerConfigurationObserverConnector
    {
    }


    public interface IConsumerConfigurator<TConsumer> :
        IPipeConfigurator<ConsumerConsumeContext<TConsumer>>,
        IConsumerConfigurationObserverConnector,
        IConsumeConfigurator
        where TConsumer : class
    {
        /// <summary>
        /// Add middleware to the message pipeline, which is invoked prior to the consumer factory.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configure">The callback to configure the message pipeline</param>
        void Message<T>(Action<IConsumerMessageConfigurator<T>> configure)
            where T : class;

        /// <summary>
        /// Add middleware to the consumer pipeline, for the specified message type, which is invoked
        /// after the consumer factory.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configure">The callback to configure the message pipeline</param>
        void ConsumerMessage<T>(Action<IConsumerMessageConfigurator<TConsumer, T>> configure)
            where T : class;
    }
}
