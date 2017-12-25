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
namespace MassTransit.RabbitMqTransport.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Scheduling;
    using Topology;
    using Util;


    public class DelayedExchangeMessageScheduler :
        IMessageScheduler
    {
        readonly IRabbitMqHostTopology _topology;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly Uri _hostAddress;

        public DelayedExchangeMessageScheduler(ISendEndpointProvider sendEndpointProvider, IRabbitMqHostTopology topology, Uri hostAddress)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _topology = topology;
            _hostAddress = hostAddress;
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return ScheduleSend(destinationAddress, scheduledTime, message, Pipe.Empty<SendContext<T>>(), cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return MessageSchedulerConverterCache.ScheduleSend(this, destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            return MessageSchedulerConverterCache.ScheduleSend(this, destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var messageType = message.GetType();

            return MessageSchedulerConverterCache.ScheduleSend(this, destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return MessageSchedulerConverterCache.ScheduleSend(this, destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            return ScheduleSend(destinationAddress, scheduledTime, message, Pipe.Empty<SendContext<T>>(), cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            return ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            return ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        Task IMessageScheduler.CancelScheduledSend(Guid tokenId)
        {
            throw new NotSupportedException("RabbitMQ delayed exchange does not support cancellation");
        }

        public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            throw new NotSupportedException("RabbitMQ delayed exchange does not support cancellation");
        }

        async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            var destinationSettings = _topology.GetSendSettings(destinationAddress);

            var sendSettings = new RabbitMqSendSettings(destinationSettings.ExchangeName + "_delay", "x-delayed-message", destinationSettings.Durable,
                destinationSettings.AutoDelete);

            sendSettings.SetExchangeArgument("x-delayed-type", destinationSettings.ExchangeType);

            sendSettings.BindToExchange(destinationSettings.ExchangeName);

            var delayExchangeAddress = sendSettings.GetSendAddress(_hostAddress);

            var delayEndpoint = await _sendEndpointProvider.GetSendEndpoint(delayExchangeAddress).ConfigureAwait(false);

            var messageId = NewId.NextGuid();

            IPipe<SendContext<T>> delayPipe = Pipe.ExecuteAsync<SendContext<T>>(async context =>
            {
                context.MessageId = messageId;
                var rabbitSendContext = context.GetPayload<RabbitMqSendContext>();

                var delay = Math.Max(0,
                    (scheduledTime.Kind == DateTimeKind.Local ? (scheduledTime - DateTime.Now) : (scheduledTime - DateTime.UtcNow)).TotalMilliseconds);

                if (delay > 0)
                    rabbitSendContext.SetTransportHeader("x-delay", (long)delay);

                await pipe.Send(context).ConfigureAwait(false);
            });

            await delayEndpoint.Send(message, delayPipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(messageId, scheduledTime, destinationAddress, message);
        }
    }
}