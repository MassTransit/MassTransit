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
namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Util;


    public class EndpointMessageScheduler :
        IMessageScheduler
    {
        readonly Func<Task<ISendEndpoint>> _schedulerEndpoint;

        public EndpointMessageScheduler(ISendEndpointProvider sendEndpointProvider, Uri schedulerAddress)
        {
            _schedulerEndpoint = () => sendEndpointProvider.GetSendEndpoint(schedulerAddress);
        }

        public EndpointMessageScheduler(ISendEndpoint sendEndpoint)
        {
            _schedulerEndpoint = () => Task.FromResult(sendEndpoint);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var pipeProxy = new ScheduleMessageContextPipe<T>(pipe);

            return ScheduleSend(destinationAddress, scheduledTime, message, pipeProxy, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var pipeProxy = new ScheduleMessageContextPipe<T>(pipe);

            return ScheduleSend(destinationAddress, scheduledTime, message, pipeProxy, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return MessageSchedulerConverterCache.ScheduleSend(this, destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            return MessageSchedulerConverterCache.ScheduleSend(this, destinationAddress, scheduledTime, message, messageType, cancellationToken);
        }

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
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

        Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
            IPipe<SendContext> pipe, CancellationToken cancellationToken)
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

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            return ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            var pipeProxy = new ScheduleMessageContextPipe<T>(pipe);

            return ScheduleSend(destinationAddress, scheduledTime, message, pipeProxy, cancellationToken);
        }

        Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            var pipeProxy = new ScheduleMessageContextPipe<T>(pipe);

            return ScheduleSend(destinationAddress, scheduledTime, message, pipeProxy, cancellationToken);
        }

        public async Task CancelScheduledSend(Guid tokenId)
        {
            var command = new CancelScheduledMessageCommand(tokenId);

            var endpoint = await _schedulerEndpoint().ConfigureAwait(false);

            await endpoint.Send<CancelScheduledMessage>(command).ConfigureAwait(false);
        }

        public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            return CancelScheduledSend(tokenId);
        }

        Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, CancellationToken cancellationToken)
            where T : class
        {
            var pipe = new ScheduleMessageContextPipe<T>();

            return ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, ScheduleMessageContextPipe<T> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            var tokenId = ScheduleTokenIdCache<T>.GetTokenId(message);

            pipe.ScheduledMessageId = tokenId;

            ScheduleMessage<T> command = new ScheduleMessageCommand<T>(scheduledTime, destinationAddress, message, tokenId);

            var endpoint = await _schedulerEndpoint().ConfigureAwait(false);

            await endpoint.Send(command, pipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(pipe.ScheduledMessageId ?? command.CorrelationId, command.ScheduledTime, command.Destination, command.Payload);
        }
    }
}