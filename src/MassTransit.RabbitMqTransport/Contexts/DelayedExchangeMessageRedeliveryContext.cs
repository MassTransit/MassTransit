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
namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Topology;


    /// <summary>
    /// Context for delaying message redelivery using a delayed RabbitMQ exchange. Requires the plug-in
    /// https://github.com/rabbitmq/rabbitmq-delayed-message-exchange
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class DelayedExchangeMessageRedeliveryContext<TMessage> :
        MessageRedeliveryContext
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;

        public DelayedExchangeMessageRedeliveryContext(ConsumeContext<TMessage> context)
        {
            _context = context;
        }

        async Task MessageRedeliveryContext.ScheduleRedelivery(TimeSpan delay)
        {
            var receiveSettings = _context.ReceiveContext.GetPayload<ReceiveSettings>();

            string delayExchangeName = receiveSettings.QueueName + "_delay";
            var sendSettings = new RabbitMqSendSettings(delayExchangeName, "x-delayed-message", receiveSettings.Durable, receiveSettings.AutoDelete);

            sendSettings.SetExchangeArgument("x-delayed-type", receiveSettings.ExchangeType);

            sendSettings.BindToQueue(receiveSettings.QueueName);

            var modelContext = _context.ReceiveContext.GetPayload<ModelContext>();

            Uri delayExchangeAddress = modelContext.ConnectionContext.HostSettings.GetSendAddress(sendSettings);

            ISendEndpoint delayEndpoint = await _context.GetSendEndpoint(delayExchangeAddress);

            await delayEndpoint.Send(_context.Message, _context.CreateCopyContextPipe((x, y) => UpdateDeliveryContext(x, y, delay)));
        }

        static void UpdateDeliveryContext(ConsumeContext context, SendContext sendContext, TimeSpan delay)
        {
            int? previousDeliveryCount = context.Headers.Get("MT-Redelivery-Count", default(int?));
            if (!previousDeliveryCount.HasValue)
                previousDeliveryCount = 0;
            sendContext.Headers.Set("MT-Redelivery-Count", previousDeliveryCount.Value + 1);

            var rabbitSendContext = sendContext.GetPayload<RabbitMqSendContext>();

            rabbitSendContext.SetTransportHeader("x-delay", (long)delay.TotalMilliseconds);
        }
    }
}