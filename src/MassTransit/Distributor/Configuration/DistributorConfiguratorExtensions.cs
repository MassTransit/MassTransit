// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System;
    using Distributor.Configuration;
    using Saga;
    using SubscriptionConfigurators;

    public static class DistributorConfiguratorExtensions
    {
        public static void Distributor(this SubscriptionBusServiceConfigurator configurator,
            Action<DistributorConfigurator> configure)
        {
            var subscriptionConfigurator = new DistributorConfiguratorImpl(configurator);

            configure(subscriptionConfigurator);
        }


        public static DistributorHandlerConfigurator<TMessage> Handler<TMessage>(
            this DistributorConfigurator configurator)
            where TMessage : class
        {
            var handlerConfigurator = new DistributorHandlerConfiguratorImpl<TMessage>();

            configurator.AddConfigurator(handlerConfigurator);

            return handlerConfigurator;
        }

        public static DistributorConsumerConfigurator<TConsumer> Consumer<TConsumer>(
            this DistributorConfigurator configurator)
            where TConsumer : class
        {
            var consumerConfigurator = new DistributorConsumerConfiguratorImpl<TConsumer>();

            configurator.AddConfigurator(consumerConfigurator);

            return consumerConfigurator;
        }

        public static DistributorSagaConfigurator<TSaga> Saga<TSaga>(
            this DistributorConfigurator configurator)
            where TSaga : class, ISaga
        {
            var consumerConfigurator = new DistributorSagaConfiguratorImpl<TSaga>();

            configurator.AddConfigurator(consumerConfigurator);

            return consumerConfigurator;
        }
    }
}