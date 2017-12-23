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
namespace MassTransit.ConsumeConnectors
{
    using System;
    using ConsumerSpecifications;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Filters;


    /// <summary>
    /// Connects a consumer instance to the inbound pipeline for the specified message type. The actual
    /// filter that invokes the consume method is passed to allow different types of message bindings,
    /// including the legacy bindings from v2.x
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class InstanceMessageConnector<TConsumer, TMessage> :
        IInstanceMessageConnector<TConsumer>
        where TConsumer : class
        where TMessage : class
    {
        readonly IFilter<ConsumerConsumeContext<TConsumer, TMessage>> _consumeFilter;

        /// <summary>
        /// Constructs the instance connector
        /// </summary>
        /// <param name="consumeFilter">The consume method invocation filter</param>
        public InstanceMessageConnector(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter)
        {
            _consumeFilter = consumeFilter;
        }

        Type IInstanceMessageConnector.MessageType => typeof(TMessage);

        ConnectHandle IInstanceMessageConnector<TConsumer>.ConnectInstance(IConsumePipeConnector pipeConnector, TConsumer instance,
            IConsumerSpecification<TConsumer> specification)
        {
            if (pipeConnector == null)
                throw new ArgumentNullException(nameof(pipeConnector));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            IConsumerMessageSpecification<TConsumer, TMessage> messageSpecification = specification.GetMessageSpecification<TMessage>();

            IPipe<ConsumerConsumeContext<TConsumer, TMessage>> consumerPipe = messageSpecification.Build(_consumeFilter);

            IPipe<ConsumeContext<TMessage>> messagePipe = messageSpecification.BuildMessagePipe(x =>
            {
                x.UseFilter(new InstanceMessageFilter<TConsumer, TMessage>(instance, consumerPipe));
            });

            return pipeConnector.ConnectConsumePipe(messagePipe);
        }

        public IConsumerMessageSpecification<TConsumer> CreateConsumerMessageSpecification()
        {
            return new ConsumerMessageSpecification<TConsumer, TMessage>();
        }
    }
}