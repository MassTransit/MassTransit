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
namespace MassTransit.Transports.RabbitMq.Pipeline
{
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Pipeline;
    using RabbitMQ.Client;

    /// <summary>
    /// Added to the model pipeline prior to the basic consumer to ensure subscriptions are bound to the consumer properly.
    /// If an subscription fails to declare or bind an exchange or queue, the consumer will not be reached
    /// </summary>
    public class SubscriptionModelFilter :
        IFilter<ModelContext>
    {
        readonly SubscriptionSettings _subscriptionSettings;

        public SubscriptionModelFilter(SubscriptionSettings subscriptionSettings)
        {
            _subscriptionSettings = subscriptionSettings;
        }

        public Task Send(ModelContext context, IPipe<ModelContext> next)
        {
            ReceiveSettings receiveSettings;
            if (!context.TryGetPayload(out receiveSettings))
                throw new PayloadNotFoundException("The ReceiveSettings were not found.");

            DeclareExchanges(context.Model);

            BindExchanges(context.Model);

            context.Model.ExchangeBind(receiveSettings.ExchangeName, _subscriptionSettings.ExchangeName, _subscriptionSettings.RoutingKey);

            return next.Send(context);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this);
        }

        void DeclareExchanges(IModel model)
        {
            foreach (ExchangeSettings exchange in _subscriptionSettings.Exchanges)
            {
                model.ExchangeDeclare(exchange.ExchangeName, exchange.ExchangeType, exchange.Durable, exchange.AutoDelete,
                    exchange.Arguments);
            }
        }

        void BindExchanges(IModel model)
        {
            foreach (ExchangeBindingSettings binding in _subscriptionSettings.Bindings)
                model.ExchangeBind(binding.Destination, binding.Source, binding.RoutingKey);
        }
    }
}