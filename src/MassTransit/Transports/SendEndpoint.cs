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
namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Pipeline;
    using Serialization;
    using Util;


    public class SendEndpoint :
        ISendEndpoint
    {
        readonly Uri _destinationAddress;
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;
        readonly ISendTransport _transport;

        public SendEndpoint(ISendTransport transport, IMessageSerializer serializer, Uri destinationAddress, Uri sourceAddress)
        {
            _transport = transport;
            _serializer = serializer;
            _destinationAddress = destinationAddress;
            _sourceAddress = sourceAddress;
        }

        public ConnectHandle Connect(ISendObserver observer)
        {
            return _transport.Connect(observer);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var settingsPipe = new EndpointSendContextPipe<T>(this);

            return _transport.Send(message, settingsPipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            var settingsPipe = new EndpointSendContextPipe<T>(this, pipe);

            return _transport.Send(message, settingsPipe, cancellationToken);
        }

        public Task Send(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Type messageType = message.GetType();

            return SendEndpointConverterCache.Send(this, message, messageType, cancellationToken);
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");

            return SendEndpointConverterCache.Send(this, message, messageType, cancellationToken);
        }

        public Task Send<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");

            T message = TypeMetadataCache<T>.InitializeFromObject(values);

            return Send(message, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            var settingsPipe = new EndpointSendContextPipe<T>(this, pipe);

            return _transport.Send(message, settingsPipe, cancellationToken);
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            Type messageType = message.GetType();

            return SendEndpointConverterCache.Send(this, message, messageType, pipe, cancellationToken);
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            return SendEndpointConverterCache.Send(this, message, messageType, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");

            T message = TypeMetadataCache<T>.InitializeFromObject(values);

            return Send(message, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            T message = TypeMetadataCache<T>.InitializeFromObject(values);

            return Send(message, pipe, cancellationToken);
        }


        class EndpointSendContextPipe<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly SendEndpoint _endpoint;
            readonly IPipe<SendContext<T>> _pipe;
            readonly IPipe<SendContext> _sendPipe;

            public EndpointSendContextPipe(SendEndpoint endpoint)
            {
                _endpoint = endpoint;
            }

            public EndpointSendContextPipe(SendEndpoint endpoint, IPipe<SendContext<T>> pipe)
            {
                _endpoint = endpoint;
                _pipe = pipe;
            }

            public EndpointSendContextPipe(SendEndpoint endpoint, IPipe<SendContext> pipe)
            {
                _endpoint = endpoint;
                _sendPipe = pipe;
            }

            public async Task Send(SendContext<T> context)
            {
                context.Serializer = _endpoint._serializer;
                context.DestinationAddress = _endpoint._destinationAddress;

                if (context.SourceAddress == null)
                    context.SourceAddress = _endpoint._sourceAddress;

                if (_pipe != null)
                    await _pipe.Send(context);
                if (_sendPipe != null)
                    await _sendPipe.Send(context);
            }

            public bool Visit(IPipeVisitor visitor)
            {
                return visitor.Visit(this,
                    x => (_pipe != null && _pipe.Visit(visitor)) || (_sendPipe != null && _sendPipe.Visit(visitor)));
            }
        }
    }
}