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
namespace MassTransit.Testing.TestDecorators
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Magnum.Reflection;
    using Pipeline;


    public class SendEndpointTestDecorator :
        ISendEndpoint
    {
        readonly ISendEndpoint _endpoint;
        readonly MessageSentListImpl _sent;

        public SendEndpointTestDecorator(ISendEndpoint endpoint, TimeSpan timeout)
        {
            _endpoint = endpoint;
            _sent = new MessageSentListImpl(timeout);
        }

        public MessageSentList Sent
        {
            get { return _sent; }
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var settingsPipe = new TestEndpointPipe<T>(this);

            return _endpoint.Send(message, settingsPipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            var settingsPipe = new TestEndpointPipe<T>(this, pipe);

            return _endpoint.Send(message, settingsPipe, cancellationToken);
        }

        public Task Send(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Type messageType = message.GetType();

            return SendEndpointConverterCache.Instance[messageType].Send(this, message, cancellationToken);
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");

            return SendEndpointConverterCache.Instance[messageType].Send(this, message, cancellationToken);
        }

        public Task Send<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");

            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            return Send(message, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            var settingsPipe = new TestEndpointPipe<T>(this, pipe);

            return _endpoint.Send(message, settingsPipe, cancellationToken);
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            Type messageType = message.GetType();

            return SendEndpointConverterCache.Instance[messageType].Send(this, message, pipe, cancellationToken);
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            return SendEndpointConverterCache.Instance[messageType].Send(this, message, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");

            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            return Send(message, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            return Send(message, pipe, cancellationToken);
        }

        void AddMessageSent(MessageSent messageSent)
        {
            _sent.Add(messageSent);
        }


        class TestEndpointPipe<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly SendEndpointTestDecorator _endpoint;
            readonly IPipe<SendContext<T>> _pipe;
            readonly IPipe<SendContext> _sendPipe;

            public TestEndpointPipe(SendEndpointTestDecorator endpoint)
            {
                _endpoint = endpoint;
            }

            public TestEndpointPipe(SendEndpointTestDecorator endpoint, IPipe<SendContext<T>> pipe)
            {
                _endpoint = endpoint;
                _pipe = pipe;
            }

            public TestEndpointPipe(SendEndpointTestDecorator endpoint, IPipe<SendContext> pipe)
            {
                _endpoint = endpoint;
                _sendPipe = pipe;
            }

            public async Task Send(SendContext<T> context)
            {
                if (_pipe != null)
                    await _pipe.Send(context);
                if (_sendPipe != null)
                    await _sendPipe.Send(context);

                var sent = new MessageSentImpl<T>(context);
                _endpoint.AddMessageSent(sent);
            }

            public bool Inspect(IPipeInspector inspector)
            {
                return inspector.Inspect(this);
            }
        }
    }
}