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


    public class SendEndpoint :
        ISendToEndpoint
    {
        readonly Uri _destinationAddress;
        readonly IMessageSendSerializer _sendSerializer;
        readonly ISendToTransport _sendToTransport;

        public SendEndpoint(ISendToTransport sendToTransport, IMessageSendSerializer sendSerializer, Uri destinationAddress)
        {
            _sendToTransport = sendToTransport;
            _sendSerializer = sendSerializer;
            _destinationAddress = destinationAddress;
        }

        public Task<SentContext<T>> Send<T>(T message)
            where T : class
        {
            return _sendToTransport.Send(message, context =>
                {
                    context.Serializer = _sendSerializer;
                    context.DestinationAddress = _destinationAddress;

                    return Task.FromResult(context);
                });
        }

        public Task<SentContext<T>> Send<T>(T message, Action<MassTransit.SendContext<T>> callback)
            where T : class
        {
            return _sendToTransport.Send(message, context =>
                {
                    context.Serializer = _sendSerializer;
                    context.DestinationAddress = _destinationAddress;

                    callback(context);

                    return Task.FromResult(context);
                });
        }

        public Task<SentContext<T>> Send<T>(T message,
            Func<MassTransit.SendContext<T>, Task<MassTransit.SendContext<T>>> callback)
            where T : class
        {
            return _sendToTransport.Send(message, async context =>
                {
                    context.Serializer = _sendSerializer;
                    context.DestinationAddress = _destinationAddress;

                    await callback(context);

                    return context;
                });
        }

        public Task<SentContext> Send(object message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var messageType = message.GetType();

            return EndpointSendConverterCache.Instance[messageType].Send(this, message);
        }

        public Task<SentContext> Send(object message, Type messageType)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");

            return EndpointSendConverterCache.Instance[messageType].Send(this, message);
        }

        public Task<SentContext> Send(object message, Action<SendContext> callback)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (callback == null)
                throw new ArgumentNullException("callback");

            var messageType = message.GetType();

            return EndpointSendConverterCache.Instance[messageType].Send(this, message, callback);
        }

        public Task<SentContext> Send(object message, Func<SendContext, Task<SendContext>> callback)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (callback == null)
                throw new ArgumentNullException("callback");

            var messageType = message.GetType();

            return EndpointSendConverterCache.Instance[messageType].Send(this, message, callback);
        }

        public Task<SentContext> Send(object message, Type messageType, Action<SendContext> callback)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            if (callback == null)
                throw new ArgumentNullException("callback");


            return EndpointSendConverterCache.Instance[messageType].Send(this, message, callback);
        }

        public Task<SentContext> Send(object message, Type messageType, Func<SendContext, Task<SendContext>> callback)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            if (callback == null)
                throw new ArgumentNullException("callback");

            return EndpointSendConverterCache.Instance[messageType].Send(this, message, callback);
        }

        public Task<SentContext<T>> Send<T>(object values)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");

            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            return Send(message);
        }

        public Task<SentContext<T>> Send<T>(object values, Action<MassTransit.SendContext<T>> callback)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");

            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            return Send(message, callback);
        }

        public Task<SentContext<T>> Send<T>(object values,
            Func<MassTransit.SendContext<T>, Task<MassTransit.SendContext<T>>> callback)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException("values");

            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            return Send(message, callback);
        }
    }
}