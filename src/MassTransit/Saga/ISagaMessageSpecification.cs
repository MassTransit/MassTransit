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
namespace MassTransit.Saga
{
    using System;
    using GreenPipes;
    using SubscriptionConfigurators;


    public interface ISagaMessageSpecification<TSaga> :
        IPipeConfigurator<SagaConsumeContext<TSaga>>,
        ISagaConfigurationObserverConnector,
        ISpecification
        where TSaga : class, ISaga
    {
        Type MessageType { get; }

        ISagaMessageSpecification<TSaga, T> GetMessageSpecification<T>()
            where T : class;
    }


    public interface ISagaMessageSpecification<TSaga, TMessage> :
        ISagaMessageSpecification<TSaga>,
        ISagaMessageConfigurator<TSaga, TMessage>,
        ISagaMessageConfigurator<TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        /// <summary>
        /// Build the consumer pipe, using the consume filter specified.
        /// </summary>
        /// <param name="consumeFilter"></param>
        /// <returns></returns>
        IPipe<SagaConsumeContext<TSaga, TMessage>> BuildConsumerPipe(IFilter<SagaConsumeContext<TSaga, TMessage>> consumeFilter);

        /// <summary>
        /// Configure the message pipe as it is built. Any previously configured filters will preceed
        /// the configuration applied by the <paramref name="configure"/> callback.
        /// </summary>
        /// <param name="configure">Configure the message pipe</param>
        /// <returns></returns>
        IPipe<ConsumeContext<TMessage>> BuildMessagePipe(Action<IPipeConfigurator<ConsumeContext<TMessage>>> configure);
    }
}