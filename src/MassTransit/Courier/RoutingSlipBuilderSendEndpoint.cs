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
namespace MassTransit.Courier
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context.Converters;
    using Contracts;
    using GreenPipes;
    using Initializers;
    using MassTransit.Pipeline.Observables;
    using Util;


    public class RoutingSlipBuilderSendEndpoint :
        ISendEndpoint
    {
        readonly string _activityName;
        readonly IRoutingSlipSendEndpointTarget _builder;
        readonly Uri _destinationAddress;
        readonly RoutingSlipEvents _events;
        readonly RoutingSlipEventContents _include;
        readonly SendObservable _observers;

        public RoutingSlipBuilderSendEndpoint(IRoutingSlipSendEndpointTarget builder, Uri destinationAddress, RoutingSlipEvents events, string activityName,
            RoutingSlipEventContents include = RoutingSlipEventContents.All)
        {
            _observers = new SendObservable();
            _builder = builder;
            _events = events;
            _activityName = activityName;
            _include = include;
            _destinationAddress = destinationAddress;
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var context = new RoutingSlipSendContext<T>(message, cancellationToken, _destinationAddress);

            _builder.AddSubscription(_destinationAddress, _events, _include, _activityName, context.GetMessageEnvelope());

            return TaskUtil.Completed;
        }

        public async Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var context = new RoutingSlipSendContext<T>(message, cancellationToken, _destinationAddress);

            await pipe.Send(context).ConfigureAwait(false);

            _builder.AddSubscription(_destinationAddress, _events, _include, _activityName, context.GetMessageEnvelope());
        }

        public async Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var context = new RoutingSlipSendContext<T>(message, cancellationToken, _destinationAddress);

            await pipe.Send(context).ConfigureAwait(false);

            _builder.AddSubscription(_destinationAddress, _events, _include, _activityName, context.GetMessageEnvelope());
        }

        public Task Send(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return SendEndpointConverterCache.Send(this, message, messageType, cancellationToken);
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            return SendEndpointConverterCache.Send(this, message, messageType, cancellationToken);
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var messageType = message.GetType();

            return SendEndpointConverterCache.Send(this, message, messageType, pipe, cancellationToken);
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return SendEndpointConverterCache.Send(this, message, messageType, pipe, cancellationToken);
        }

        public Task Send<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return MessageInitializerCache<T>.Send(this, values, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return MessageInitializerCache<T>.Send(this, values, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return MessageInitializerCache<T>.Send(this, values, pipe, cancellationToken);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}
