// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    /// Using the delayed exchange plug-in allows messages to be scheduled. This is used to
    /// send scheduled messages to that exchange
    /// </summary>
    public class DelayedExchangeMessageSchedulerContext :
        MessageSchedulerContext
    {
        readonly ConsumeContext _consumeContext;

        public DelayedExchangeMessageSchedulerContext(ConsumeContext consumeContext)
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
            var destinationSettings = destinationAddress.GetSendSettings();

            var sendSettings = new RabbitMqSendSettings(destinationSettings.ExchangeName + "_delay", "x-delayed-message", destinationSettings.Durable,
                destinationSettings.AutoDelete);

            sendSettings.SetExchangeArgument("x-delayed-type", destinationSettings.ExchangeType);

            sendSettings.BindToExchange(destinationSettings.ExchangeName);

            var modelContext = _consumeContext.ReceiveContext.GetPayload<ModelContext>();

            var delayExchangeAddress = modelContext.ConnectionContext.HostSettings.GetSendAddress(sendSettings);

            var delayEndpoint = await _consumeContext.GetSendEndpoint(delayExchangeAddress).ConfigureAwait(false);

            var messageId = NewId.NextGuid();

            IPipe<SendContext> delayPipe = Pipe.New<SendContext>(x =>
            {
                x.UseExecuteAsync(async context =>
                {
                    context.MessageId = messageId;
                    var rabbitSendContext = context.GetPayload<RabbitMqSendContext>();

                    var delay = Math.Max(0, (deliveryTime.Kind == DateTimeKind.Local ? (deliveryTime - DateTime.Now) : (deliveryTime - DateTime.UtcNow)).TotalMilliseconds);

                    if(delay>0)
                        rabbitSendContext.SetTransportHeader("x-delay", (long)delay);

                    await sendPipe.Send(context).ConfigureAwait(false);
                });
            });

            await delayEndpoint.Send(message, delayPipe).ConfigureAwait(false);

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
            throw new NotImplementedByDesignException("RabbitMQ delayed exchanges do not support cancelling scheduled messages.");
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