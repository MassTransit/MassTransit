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
namespace MassTransit.Context
{
    using System;
    using System.IO;

    public class SendContext<T> :
        MessageContext,
        ISendContext<T>
        where T : class
    {
        readonly T _message;
        Action<Stream> _bodyWriter;
        Guid _id;
        IReceiveContext _receiveContext;

        SendContext(Guid messageId, T message, Type declaringMessageType)
        {
            _id = messageId;
            _message = message;

            SetMessageId(_id.ToString());
            this.SetMessageType(typeof(T));
            DeclaringMessageType = declaringMessageType;
        }

        public SendContext(T message)
            : this(NewId.NextGuid(), message, typeof(T))
        {
        }

        protected SendContext(T message, ISendContext context)
            : this(context.Id, message, context.DeclaringMessageType)
        {
            SetMessageId(_id.ToString());

            SetUsing(context);

            // need to reset this since SetUsing copies the context value
            this.SetMessageType(typeof (T));

            DeliveryMode = context.DeliveryMode;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public Type DeclaringMessageType { get; private set; }

        public void SetDeliveryMode(DeliveryMode deliveryMode)
        {
            DeliveryMode = deliveryMode;
        }

        public DeliveryMode DeliveryMode { get; private set; }

        public void SerializeTo(Stream stream)
        {
            if (_bodyWriter == null)
                throw new InvalidOperationException("No message body writer was specified");

            _bodyWriter(stream);
        }

        public virtual bool TryGetContext<TMessage>(out IBusPublishContext<TMessage> context)
            where TMessage : class
        {
            context = PublishContext<TMessage>.FromMessage(_message, this);

            return context != null;
        }

        public virtual void NotifySend(IEndpointAddress address)
        {
            if (_receiveContext != null)
                _receiveContext.NotifySend(this, address);
        }

        public T Message
        {
            get { return _message; }
        }

        public void SetBodyWriter(Action<Stream> bodyWriter)
        {
            _bodyWriter = bodyWriter;
        }

        public void SetReceiveContext(IReceiveContext receiveContext)
        {
            _receiveContext = receiveContext;
        }
    }
}