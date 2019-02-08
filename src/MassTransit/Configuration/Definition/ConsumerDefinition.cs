// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Definition
{
    using System;
    using ConsumeConfigurators;
    using GreenPipes;
    using GreenPipes.Filters;


    /// <summary>
    /// A consumer definition defines the configuration for a consumer, which can be used by the automatic registration code to
    /// configure the consumer on a receive endpoint.
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    public class ConsumerDefinition<TConsumer> :
        IConsumerDefinition<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly Lazy<IFilter<ConsumerConsumeContext<TConsumer>>> _filter;
        int? _concurrencyLimit;
        string _endpointName;

        protected ConsumerDefinition()
        {
            // TODO this needs to observe messages types and use a shared limit across all message types
            _filter = new Lazy<IFilter<ConsumerConsumeContext<TConsumer>>>(() =>
                new ConcurrencyLimitFilter<ConsumerConsumeContext<TConsumer>>(_concurrencyLimit.Value));

            // TODO if the partitionKey is specified, use a partition filter instead of a semaphore
        }

        /// <summary>
        /// Specify the endpoint name (which may be a queue, or a subscription, depending upon the transport) on which the consumer
        /// should be configured.
        /// </summary>
        protected string EndpointName
        {
            set => _endpointName = value;
        }

        protected int ConcurrencyLimit
        {
            set => _concurrencyLimit = value;
        }

        void IConsumerDefinition<TConsumer>.Configure(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TConsumer> consumerConfigurator)
        {
            if (_concurrencyLimit.HasValue)
                consumerConfigurator.UseFilter(_filter.Value);

            ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }

        public Type ConsumerType => typeof(TConsumer);

        string IConsumerDefinition.GetEndpointName(IEndpointNameFormatter formatter)
        {
            return !string.IsNullOrWhiteSpace(_endpointName)
                ? _endpointName
                : formatter.Consumer<TConsumer>();
        }

        /// <summary>
        /// Define a message handled by the consumer
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        protected void Message<T>(Action<IConsumerMessageDefinitionConfigurator<TConsumer, T>> configure = null)
            where T : class
        {
        }

        /// <summary>
        /// Define the request message handled by the consumer
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        protected void Request<T>(Action<IConsumerRequestDefinitionConfigurator<TConsumer, T>> configure = null)
            where T : class
        {
        }

        /// <summary>
        /// Called when the consumer is being configured on the endpoint. Configuration only applies to this consumer, and does not apply to
        /// the endpoint.
        /// </summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="consumerConfigurator">The consumer configurator</param>
        protected virtual void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TConsumer> consumerConfigurator)
        {
        }
    }
}
