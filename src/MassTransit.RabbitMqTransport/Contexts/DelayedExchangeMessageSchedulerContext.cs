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
    using MassTransit.Pipeline;
    using Scheduling;
    using Topology;


    /// <summary>
    /// Uses the RabbitMQ delayed exchange feature to schedule message delivery
    /// </summary>
    public class DelayedExchangeMessageSchedulerContext :
        MessageSchedulerContext
    {
        readonly ConsumeContext _context;

        public DelayedExchangeMessageSchedulerContext(ConsumeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(T message, TimeSpan deliveryDelay, IPipe<SendContext> sendPipe)
            where T : class
        {
            return ScheduleSend(message, _context.ReceiveContext.InputAddress, deliveryDelay, sendPipe);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(T message, DateTime deliveryTime, IPipe<SendContext> sendPipe)
            where T : class
        {
            return ScheduleSend(message, _context.ReceiveContext.InputAddress, deliveryTime, sendPipe);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(T message, Uri destinationAddress, TimeSpan deliveryDelay, IPipe<SendContext> sendPipe)
            where T : class
        {
            var scheduledTime = DateTime.UtcNow + deliveryDelay;

            return ScheduleSend(message, destinationAddress, scheduledTime, sendPipe);
        }

        public async Task<ScheduledMessage<T>> ScheduleSend<T>(T message, Uri destinationAddress, DateTime deliveryTime, IPipe<SendContext> sendPipe)
            where T : class
        {
            var endpoint = await _context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            var messageId = NewId.NextGuid();
            await endpoint.Send(message, context =>
            {
                context.MessageId = messageId;
                context.SetScheduledEnqueueTime(deliveryTime);
            }).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(messageId, deliveryTime, destinationAddress, message);
        }


        async Task ScheduleRedelivery(TimeSpan delay)
        {
            var receiveSettings = _context.ReceiveContext.GetPayload<ReceiveSettings>();

            var delayExchangeAddress = GetDelayExchangeAddress(receiveSettings);

            ISendEndpoint delayEndpoint = await _context.GetSendEndpoint(delayExchangeAddress).ConfigureAwait(false);

            await delayEndpoint.Send(_context.Message, _context.CreateCopyContextPipe((x, y) => UpdateDeliveryContext(x, y, delay))).ConfigureAwait(false);
        }

        Uri GetDelayExchangeAddress(Uri destinationAddress)
        {
            var destinationSendSettings = destinationAddress.GetSendSettings();



            modelContext.ConnectionContext.HostSettings.
            var receiveSettings = _context.ReceiveContext.GetPayload<ReceiveSettings>();

            string delayExchangeName = destinationSendSettings.ExchangeName + "_delay";
            var sendSettings = new RabbitMqSendSettings(delayExchangeName, "x-delayed-message", destinationSendSettings.Durable, destinationSendSettings.AutoDelete);

            sendSettings.SetExchangeArgument("x-delayed-type", destinationSendSettings.ExchangeType);

            sendSettings.BindToQueue(receiveSettings.QueueName);

            var modelContext = _context.ReceiveContext.GetPayload<ModelContext>();

            return modelContext.ConnectionContext.HostSettings.GetSendAddress(sendSettings);
        }

        public Task CancelScheduledSend<T>(ScheduledMessage<T> message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return CancelScheduledSend(message.TokenId);
        }

        public Task CancelScheduledSend(Guid tokenId)
        {
            throw new NotImplementedByDesignException("ServiceBus does not support cancelling scheduled messages.");
        }


        class ScheduledMessageHandle<T> :
            ScheduledMessage<T>
            where T : class
        {
            public ScheduledMessageHandle(Guid tokenId, DateTime scheduledTime, Uri destination, T payload)
            {
                TokenId = tokenId;
                ScheduledTime = scheduledTime;
                Destination = destination;
                Payload = payload;
            }

            public Guid TokenId { get; }
            public DateTime ScheduledTime { get; }
            public Uri Destination { get; }
            public T Payload { get; }
        }
    }
}