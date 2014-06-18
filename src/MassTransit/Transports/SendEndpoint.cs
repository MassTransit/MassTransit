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
    using System.Threading.Tasks;
    using Context;
    using Magnum.Reflection;
    using Pipeline;


    public class SendEndpoint :
        ISendToEndpoint
    {
        readonly Uri _destinationAddress;
        readonly ISendMessageSerializer _serializer;
        readonly ISendTransport _transport;

        public SendEndpoint(ISendTransport transport, ISendMessageSerializer serializer, Uri destinationAddress)
        {
            _transport = transport;
            _serializer = serializer;
            _destinationAddress = destinationAddress;
        }


        public Task Send<T>(T message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var settingsPipe = new EndpointSettingsPipe<T>(this);

            return _transport.Send(message, settingsPipe);
        }

        public Task Send<T>(T message, ISendPipe pipe) 
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            var settingsPipe = new EndpointSettingsPipe<T>(this, pipe);

            return _transport.Send(message, settingsPipe);
        }

        public Task Send(object message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Type messageType = message.GetType();

            return EndpointSendConverterCache.Instance[messageType].Send(this, message);
        }

        public Task Send(object message, Type messageType)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");

            return EndpointSendConverterCache.Instance[messageType].Send(this, message);
        }

        public Task Send<T>(object values)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");

            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            return Send(message);
        }

        public Task Send<T>(T message, ISendPipe<T> pipe)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            var settingsPipe = new EndpointSettingsPipe<T>(this, pipe);

            return _transport.Send(message, settingsPipe);
        }

        public Task Send(object message, ISendPipe pipe)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            Type messageType = message.GetType();

            return EndpointSendConverterCache.Instance[messageType].Send(this, message, pipe);
        }

        public Task Send(object message, Type messageType, ISendPipe pipe)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            return EndpointSendConverterCache.Instance[messageType].Send(this, message, pipe);
        }

        public Task Send<T>(object values, ISendPipe<T> pipe)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");

            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            return Send(message, pipe);
        }

        public Task Send<T>(object values, ISendPipe pipe) 
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (pipe == null)
                throw new ArgumentNullException("pipe");

            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            return Send(message, pipe);
        }


        class EndpointSettingsPipe<T> :
            ISendPipe<T>
            where T : class
        {
            readonly SendEndpoint _endpoint;
            readonly ISendPipe<T> _pipe;
            readonly ISendPipe _sendPipe;

            public EndpointSettingsPipe(SendEndpoint endpoint)
            {
                _endpoint = endpoint;
            }

            public EndpointSettingsPipe(SendEndpoint endpoint, ISendPipe<T> pipe)
            {
                _endpoint = endpoint;
                _pipe = pipe;
            }

            public EndpointSettingsPipe(SendEndpoint endpoint, ISendPipe pipe)
            {
                _endpoint = endpoint;
                _sendPipe = pipe;
            }

            public async Task Send(MassTransit.SendContext<T> context)
            {
                context.Serializer = _endpoint._serializer;
                context.DestinationAddress = _endpoint._destinationAddress;

                if (_pipe != null)
                    await _pipe.Send(context);
                if (_sendPipe != null)
                    await _sendPipe.Send(context);
            }

            public bool Inspect(IPipeInspector inspector)
            {
                return inspector.Inspect(this);
            }
        }
    }
}