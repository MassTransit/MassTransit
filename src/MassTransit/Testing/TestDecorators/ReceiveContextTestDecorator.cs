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
    using System.Collections.Generic;
    using System.IO;
    using Context;
    using Scenarios;
    using Serialization;

    public class ReceiveContextTestDecorator :
        IReceiveContext
    {
        readonly IReceiveContext _context;
        readonly EndpointTestScenarioImpl _scenario;

        public ReceiveContextTestDecorator(IReceiveContext context, EndpointTestScenarioImpl scenario)
        {
            _context = context;
            _scenario = scenario;
        }

        public void SetHeader(string key, string value)
        {
            _context.SetHeader(key, value);
        }

        public void SetRequestId(string value)
        {
            _context.SetRequestId(value);
        }

        public void SetConversationId(string value)
        {
            _context.SetConversationId(value);
        }

        public void SetCorrelationId(string value)
        {
            _context.SetCorrelationId(value);
        }

        public void SetOriginalMessageId(string value)
        {
            _context.SetOriginalMessageId(value);
        }

        public string RequestId
        {
            get { return _context.RequestId; }
        }

        public string ConversationId
        {
            get { return _context.ConversationId; }
        }

        public string CorrelationId
        {
            get { return _context.CorrelationId; }
        }

        public string MessageId
        {
            get { return _context.MessageId; }
        }

        public string MessageType
        {
            get { return _context.MessageType; }
        }

        public string ContentType
        {
            get { return _context.ContentType; }
        }

        public Uri SourceAddress
        {
            get { return _context.SourceAddress; }
        }

        public Uri InputAddress
        {
            get { return _context.InputAddress; }
        }

        public Uri DestinationAddress
        {
            get { return _context.DestinationAddress; }
        }

        public Uri ResponseAddress
        {
            get { return _context.ResponseAddress; }
        }

        public Uri FaultAddress
        {
            get { return _context.FaultAddress; }
        }

        public string Network
        {
            get { return _context.Network; }
        }

        public DateTime? ExpirationTime
        {
            get { return _context.ExpirationTime; }
        }

        public int RetryCount
        {
            get { return _context.RetryCount; }
        }

        public IMessageHeaders Headers
        {
            get { return _context.Headers; }
        }

        public IReceiveContext BaseContext
        {
            get { return _context.BaseContext; }
        }

        public IServiceBus Bus
        {
            get { return _context.Bus; }
        }

        public IEndpoint Endpoint
        {
            get { return _context.Endpoint; }
        }

        public bool IsContextAvailable(Type messageType)
        {
            return _context.IsContextAvailable(messageType);
        }

        public bool TryGetContext<T>(out IConsumeContext<T> context) 
            where T : class
        {
            return _context.TryGetContext(out context);
        }

        public void Respond<T>(T message, Action<ISendContext<T>> contextCallback) 
            where T : class
        {
            _context.Respond(message, contextCallback);
        }

        public void SetContentType(string value)
        {
            _context.SetContentType(value);
        }

        public void SetMessageId(string value)
        {
            _context.SetMessageId(value);
        }

        public void SetInputAddress(Uri uri)
        {
            _context.SetInputAddress(uri);
        }

        public void SetEndpoint(IEndpoint endpoint)
        {
            _context.SetEndpoint(endpoint);
        }

        public void SetBus(IServiceBus bus)
        {
            IServiceBus busValue = _scenario.GetDecoratedBus(bus);

            _context.SetBus(busValue);
        }

        public void SetSourceAddress(Uri uri)
        {
            _context.SetSourceAddress(uri);
        }

        public void SetDestinationAddress(Uri uri)
        {
            _context.SetDestinationAddress(uri);
        }

        public void SetResponseAddress(Uri uri)
        {
            _context.SetResponseAddress(uri);
        }

        public void SetFaultAddress(Uri uri)
        {
            _context.SetFaultAddress(uri);
        }

        public void SetNetwork(string value)
        {
            _context.SetNetwork(value);
        }

        public void SetRetryCount(int retryCount)
        {
            _context.SetRetryCount(retryCount);
        }

        public void SetExpirationTime(DateTime value)
        {
            _context.SetExpirationTime(value);
        }

        public void SetMessageType(string messageType)
        {
            _context.SetMessageType(messageType);
        }

        public void SetBodyStream(Stream stream)
        {
            _context.SetBodyStream(stream);
        }

        public void CopyBodyTo(Stream stream)
        {
            _context.CopyBodyTo(stream);
        }

        public Stream BodyStream
        {
            get { return _context.BodyStream; }
        }

        public void SetMessageTypeConverter(IMessageTypeConverter messageTypeConverter)
        {
            _context.SetMessageTypeConverter(messageTypeConverter);
        }

        public void NotifyFault(Action faultAction)
        {
            _context.NotifyFault(faultAction);
        }

        public void NotifySend(ISendContext context, IEndpointAddress address)
        {
            _context.NotifySend(context, address);
        }

        public void NotifySend<T>(ISendContext<T> sendContext, IEndpointAddress address) where T : class
        {
            _context.NotifySend(sendContext, address);
        }

        public void NotifyPublish<T>(IPublishContext<T> publishContext) where T : class
        {
            _context.NotifyPublish(publishContext);
        }

        public void NotifyConsume<T>(IConsumeContext<T> consumeContext, string consumerType, string correlationId)
            where T : class
        {
            _context.NotifyConsume(consumeContext, consumerType, correlationId);
        }

        public void ExecuteFaultActions(IEnumerable<Action> faultActions)
        {
            _context.ExecuteFaultActions(faultActions);
        }

        public IEnumerable<ISent> Sent
        {
            get { return _context.Sent; }
        }

        public IEnumerable<IReceived> Received
        {
            get { return _context.Received; }
        }

        public Guid Id
        {
            get { return _context.Id; }
        }

        public bool IsTransactional
        {
            get { return _context.IsTransactional; }
        }

        public string OriginalMessageId
        {
            get { return _context.OriginalMessageId; }
        }

        public IEnumerable<Action> GetFaultActions()
        {
            return _context.GetFaultActions();
        }
    }
}