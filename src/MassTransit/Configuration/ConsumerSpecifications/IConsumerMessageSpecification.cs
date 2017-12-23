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
namespace MassTransit.ConsumerSpecifications
{
    using System;
    using ConsumeConfigurators;
    using GreenPipes;


    public interface IConsumerMessageSpecification<TConsumer> :
        IPipeConfigurator<ConsumerConsumeContext<TConsumer>>,
        IConsumerConfigurationObserverConnector,
        ISpecification
        where TConsumer : class
    {
        Type MessageType { get; }

        IConsumerMessageSpecification<TConsumer, T> GetMessageSpecification<T>()
            where T : class;
    }


    public interface IConsumerMessageSpecification<TConsumer, TMessage> :
        IConsumerMessageSpecification<TConsumer>,
        IConsumerMessageConfigurator<TConsumer, TMessage>,
        IConsumerMessageConfigurator<TMessage>
        where TConsumer : class
        where TMessage : class
    {
        IPipe<ConsumerConsumeContext<TConsumer, TMessage>> Build(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter);

        /// <summary>
        /// Configure the message pipe as it is built. Any previously configured filters will preceed
        /// the configuration applied by the <paramref name="configure"/> callback.
        /// </summary>
        /// <param name="configure">Configure the message pipe</param>
        /// <returns></returns>
        IPipe<ConsumeContext<TMessage>> BuildMessagePipe(Action<IPipeConfigurator<ConsumeContext<TMessage>>> configure);
    }
}