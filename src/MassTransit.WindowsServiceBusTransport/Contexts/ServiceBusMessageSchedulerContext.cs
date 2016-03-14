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
namespace MassTransit.WindowsServiceBusTransport.Contexts
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Scheduling;


    /// <summary>
    /// ServiceBus can schedule a message for delivery, so leverage that ability to schedule messages
    /// instead of relying on an external scheduler.
    /// </summary>
    public class ServiceBusMessageSchedulerContext :
        MessageSchedulerContext
    {
        readonly ConsumeContext _consumeContext;

        public ServiceBusMessageSchedulerContext(ConsumeContext consumeContext)
        {
            if (consumeContext == null)
                throw new ArgumentNullException(nameof(consumeContext));

            _consumeContext = consumeContext;
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(T message, TimeSpan deliveryDelay, IPipe<SendContext> sendPipe)
            where T : class
        {
            return ScheduleSend(message, _consumeContext.ReceiveContext.InputAddress, deliveryDelay, sendPipe);
        }

        public Task<ScheduledMessage<T>> ScheduleSend<T>(T message, DateTime deliveryTime, IPipe<SendContext> sendPipe)
            where T : class
        {
            return ScheduleSend(message, _consumeContext.ReceiveContext.InputAddress, deliveryTime, sendPipe);
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
            var endpoint = await _consumeContext.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            var messageId = NewId.NextGuid();
            await endpoint.Send(message, context =>
            {
                context.MessageId = messageId;
                context.SetScheduledEnqueueTime(deliveryTime);
            }).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(messageId, deliveryTime, destinationAddress, message);
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