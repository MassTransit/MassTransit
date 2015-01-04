// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System.Threading.Tasks;
    using MassTransit.Pipeline;


    /// <summary>
    /// Added to the model pipeline prior to the basic consumer to ensure subscriptions are bound to the consumer properly.
    /// If an subscription fails to declare or bind an exchange or queue, the consumer will not be reached
    /// </summary>
    public class ReceiveExchangeBindingModelFilter :
        IFilter<ModelContext>
    {
        readonly ExchangeBindingSettings _exchangeBindingSettings;

        public ReceiveExchangeBindingModelFilter(ExchangeBindingSettings exchangeBindingSettings)
        {
            _exchangeBindingSettings = exchangeBindingSettings;
        }

        Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            ReceiveSettings receiveSettings;
            if (!context.TryGetPayload(out receiveSettings))
                throw new PayloadNotFoundException("The ReceiveSettings were not found.");

            ExchangeSettings exchange = _exchangeBindingSettings.Exchange;

            context.Model.ExchangeDeclare(exchange.ExchangeName, exchange.ExchangeType, exchange.Durable, exchange.AutoDelete,
                exchange.Arguments);

            context.Model.ExchangeBind(receiveSettings.ExchangeName, exchange.ExchangeName, _exchangeBindingSettings.RoutingKey);

            return next.Send(context);
        }

        public bool Visit(IPipeVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}