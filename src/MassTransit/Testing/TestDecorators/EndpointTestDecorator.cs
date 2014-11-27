// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Context;
    using Magnum.Reflection;
    using Scenarios;
    using Serialization;
    using Transports;


    public class EndpointTestDecorator :
        IEndpoint
    {
        readonly IEndpoint _endpoint;
        readonly ReceivedMessageListImpl _received;
        readonly SentMessageListImpl _sent;
        EndpointTestScenarioImpl _scenario;

        public EndpointTestDecorator(IEndpoint endpoint, EndpointTestScenarioImpl scenario)
        {
            _endpoint = endpoint;
            _scenario = scenario;

            _sent = new SentMessageListImpl();
            _received = new ReceivedMessageListImpl();
        }

        public ReceivedMessageList Received
        {
            get { return _received; }
        }

        public SentMessageList Sent
        {
            get { return _sent; }
        }

        public void Dispose()
        {
            _endpoint.Dispose();
            _received.Dispose();
            _sent.Dispose();
        }

        public EndpointAddress Address
        {
            get { return _endpoint.Address; }
        }

        public IInboundTransport InboundTransport
        {
            get { return _endpoint.InboundTransport; }
        }

        public IOutboundTransport OutboundTransport
        {
            get { return _endpoint.OutboundTransport; }
        }

        public IOutboundTransport ErrorTransport
        {
            get { return _endpoint.ErrorTransport; }
        }

        public IMessageSerializer Serializer
        {
            get { return _endpoint.Serializer; }
        }

        public void Send<T>(ISendContext<T> context) 
            where T : class
        {
            var send = new SentMessageImpl<T>(context);
            try
            {
                _endpoint.Send(context);
            }
            catch (Exception ex)
            {
                send.SetException(ex);
                throw;
            }
            finally
            {
                _sent.Add(send);
                _scenario.AddSent(send);
            }
        }

        public void Send<T>(T message)
            where T : class
        {
            ISendContext<T> context = ContextStorage.CreateSendContext(message);

            Send(context);
        }

        public void Send<T>(T message, Action<ISendContext<T>> contextCallback)
            where T : class
        {
            ISendContext<T> context = ContextStorage.CreateSendContext(message);

            contextCallback(context);

            Send(context);
        }

        public void Send(object message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            SendObjectConverterCache.Instance[message.GetType()].Send(this, message);
        }

        public void Send(object message, Type messageType)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");

            SendObjectConverterCache.Instance[messageType].Send(this, message);
        }

        public void Send(object message, Action<ISendContext> contextCallback)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (contextCallback == null)
                throw new ArgumentNullException("contextCallback");

            Type messageType = message.GetType();

            SendObjectConverterCache.Instance[messageType].Send(this, message, contextCallback);
        }

        public void Send(object message, Type messageType, Action<ISendContext> contextCallback)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            if (contextCallback == null)
                throw new ArgumentNullException("contextCallback");

            SendObjectConverterCache.Instance[messageType].Send(this, message, contextCallback);
        }

        public void Send<T>(object values) where T : class
        {
            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            Send(message, x => { });
        }

        public void Send<T>(object values, Action<ISendContext<T>> contextCallback) where T : class
        {
            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            Send(message, contextCallback);
        }

        public void Receive(Func<IReceiveContext, Action<IReceiveContext>> receiver, TimeSpan timeout)
        {
            _endpoint.Receive(receiveContext =>
                {
                    var decoratedReceiveContext = new ReceiveContextTestDecorator(receiveContext, _scenario);

                    Action<IReceiveContext> receive = receiver(decoratedReceiveContext);
                    if (receive == null)
                    {
                        var skipped = new ReceivedMessageImpl(receiveContext);
                        _scenario.AddSkipped(skipped);

                        return null;
                    }

                    return context =>
                        {
                            decoratedReceiveContext = new ReceiveContextTestDecorator(context, _scenario);
                            var received = new ReceivedMessageImpl(context);
                            try
                            {
                                receive(decoratedReceiveContext);
                            }
                            catch (Exception ex)
                            {
                                received.SetException(ex);
                                throw;
                            }
                            finally
                            {
                                _received.Add(received);
                                _scenario.AddReceived(received);
                            }
                        };
                }, timeout);
        }
    }
}