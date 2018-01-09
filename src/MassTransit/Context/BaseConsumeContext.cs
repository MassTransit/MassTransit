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
namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Events;
    using GreenPipes;
    using Pipeline.Pipes;
    using Serialization;
    using Util;


    public abstract class BaseConsumeContext :
        BasePipeContext,
        ConsumeContext
    {
        readonly Lazy<IPublishEndpoint> _publishEndpoint;
        readonly ReceiveContext _receiveContext;

        protected BaseConsumeContext(ReceiveContext receiveContext)
            : base(receiveContext)
        {
            _receiveContext = receiveContext;

            _publishEndpoint = new Lazy<IPublishEndpoint>(() =>
                receiveContext.PublishEndpointProvider.CreatePublishEndpoint(_receiveContext.InputAddress, CorrelationId, ConversationId));
        }

        public ReceiveContext ReceiveContext => _receiveContext;
        public Task CompleteTask => _receiveContext.CompleteTask;

        public abstract Guid? MessageId { get; }
        public abstract Guid? RequestId { get; }
        public abstract Guid? CorrelationId { get; }
        public abstract Guid? ConversationId { get; }
        public abstract Guid? InitiatorId { get; }
        public abstract DateTime? ExpirationTime { get; }
        public abstract Uri SourceAddress { get; }
        public abstract Uri DestinationAddress { get; }
        public abstract Uri ResponseAddress { get; }
        public abstract Uri FaultAddress { get; }
        public abstract Headers Headers { get; }
        public abstract HostInfo Host { get; }
        public abstract IEnumerable<string> SupportedMessageTypes { get; }
        public abstract bool HasMessageType(Type messageType);
        public abstract bool TryGetMessage<T>(out ConsumeContext<T> consumeContext) where T : class;

        public async Task RespondAsync<T>(T message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (ResponseAddress != null)
            {
                var endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await endpoint.Send(message, new ResponsePipe<T>(this), CancellationToken).ConfigureAwait(false);
            }
            else
                await _publishEndpoint.Value.Publish(message, new ResponsePipe<T>(this), CancellationToken).ConfigureAwait(false);
        }

        public async Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (sendPipe == null)
                throw new ArgumentNullException(nameof(sendPipe));

            if (ResponseAddress != null)
            {
                var endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await endpoint.Send(message, new ResponsePipe<T>(this, sendPipe), CancellationToken).ConfigureAwait(false);
            }
            else
                await _publishEndpoint.Value.Publish(message, new ResponsePipe<T>(this, sendPipe), CancellationToken).ConfigureAwait(false);
        }

        public async Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (sendPipe == null)
                throw new ArgumentNullException(nameof(sendPipe));

            if (ResponseAddress != null)
            {
                var endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await endpoint.Send(message, new ResponsePipe<T>(this, sendPipe), CancellationToken).ConfigureAwait(false);
            }
            else
                await _publishEndpoint.Value.Publish(message, new ResponsePipe<T>(this, sendPipe), CancellationToken).ConfigureAwait(false);
        }

        public Task RespondAsync(object message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return RespondAsync(message, messageType);
        }

        public async Task RespondAsync(object message, Type messageType)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            if (ResponseAddress != null)
            {
                var endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await SendEndpointConverterCache.Send(endpoint, message, messageType, new ResponsePipe(this), CancellationToken).ConfigureAwait(false);
            }
            else
                await _publishEndpoint.Value.Publish(message, new ResponsePipe(this), CancellationToken).ConfigureAwait(false);
        }

        public Task RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (sendPipe == null)
                throw new ArgumentNullException(nameof(sendPipe));

            var messageType = message.GetType();

            return RespondAsync(message, messageType, sendPipe);
        }

        public async Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));
            if (sendPipe == null)
                throw new ArgumentNullException(nameof(sendPipe));

            if (ResponseAddress != null)
            {
                var endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await SendEndpointConverterCache.Send(endpoint, message, messageType, new ResponsePipe(this, sendPipe), CancellationToken).ConfigureAwait(false);
            }
            else
                await _publishEndpoint.Value.Publish(message, new ResponsePipe(this, sendPipe), CancellationToken).ConfigureAwait(false);
        }

        public Task RespondAsync<T>(object values) where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            return RespondAsync(message);
        }

        public Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe) where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            return RespondAsync(message, sendPipe);
        }

        public Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe) where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            return RespondAsync(message, sendPipe);
        }

        public void Respond<T>(T message)
            where T : class
        {
            var task = RespondAsync(message);

            _receiveContext.AddPendingTask(task);
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            var sendEndpoint = await _receiveContext.SendEndpointProvider.GetSendEndpoint(address).ConfigureAwait(false);

            return new ConsumeSendEndpoint(sendEndpoint, this);
        }

        public Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            return _receiveContext.NotifyConsumed(context, duration, consumerType);
        }

        public Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            var faultTask = GenerateFault(context, exception);

            _receiveContext.AddPendingTask(faultTask);

            return _receiveContext.NotifyFaulted(context, duration, consumerType, exception);
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken) where T : class
        {
            var task = _publishEndpoint.Value.Publish(message, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken) where T : class
        {
            var task = _publishEndpoint.Value.Publish(message, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken) where T : class
        {
            var task = _publishEndpoint.Value.Publish(message, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        public Task Publish(object message, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish(message, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish(message, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish(message, messageType, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish(message, messageType, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken) where T : class
        {
            var task = _publishEndpoint.Value.Publish<T>(values, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken) where T : class
        {
            var task = _publishEndpoint.Value.Publish(values, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken) where T : class
        {
            var task = _publishEndpoint.Value.Publish<T>(values, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishEndpoint.Value.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _receiveContext.SendEndpointProvider.ConnectSendObserver(observer);
        }

        async Task GenerateFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            Fault<T> fault = new FaultEvent<T>(context.Message, context.MessageId, HostMetadataCache.Host, exception);

            IPipe<SendContext<Fault<T>>> faultPipe = Pipe.Execute<SendContext<Fault<T>>>(x =>
            {
                x.SourceAddress = ReceiveContext.InputAddress;
                x.CorrelationId = CorrelationId;
                x.RequestId = RequestId;

                foreach (KeyValuePair<string, object> header in Headers.GetAll())
                    x.Headers.Set(header.Key, header.Value);
            });

            if (FaultAddress != null)
            {
                var endpoint = await GetSendEndpoint(FaultAddress).ConfigureAwait(false);

                await endpoint.Send(fault, faultPipe, CancellationToken).ConfigureAwait(false);
            }
            else if (ResponseAddress != null)
            {
                var endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await endpoint.Send(fault, faultPipe, CancellationToken).ConfigureAwait(false);
            }
            else
                await _publishEndpoint.Value.Publish(fault, faultPipe, CancellationToken).ConfigureAwait(false);
        }
    }
}