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
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Threading;
    using GreenPipes;
    using GreenPipes.Payloads;


    public abstract class BaseSendContext<TMessage> :
        BasePipeContext,
        SendContext<TMessage>
        where TMessage : class
    {
        readonly DictionarySendHeaders _headers;
        byte[] _body;
        IMessageSerializer _serializer;

        protected BaseSendContext(TMessage message, CancellationToken cancellationToken)
            : base(new PayloadCache(), cancellationToken)
        {
            Message = message;

            _headers = new DictionarySendHeaders();

            MessageId = NewId.NextGuid();
            Durable = true;
        }

        public byte[] Body
        {
            get
            {
                if (_body != null)
                    return _body;

                if (Serializer == null)
                    throw new SerializationException("No serializer specified");
                if (Message == null)
                    throw new SendException(typeof(TMessage), DestinationAddress, "No message specified");

                using (var memoryStream = new MemoryStream(8192))
                {
                    Serializer.Serialize(memoryStream, this);

                    _body = memoryStream.ToArray();
                    return _body;
                }
            }
        }

        public Guid? MessageId { get; set; }
        public Guid? RequestId { get; set; }
        public Guid? CorrelationId { get; set; }

        public Guid? ConversationId { get; set; }
        public Guid? InitiatorId { get; set; }

        public Guid? ScheduledMessageId { get; set; }

        public SendHeaders Headers => _headers;

        public Uri SourceAddress { get; set; }
        public Uri DestinationAddress { get; set; }
        public Uri ResponseAddress { get; set; }
        public Uri FaultAddress { get; set; }

        public TimeSpan? TimeToLive { get; set; }

        public ContentType ContentType { get; set; }

        public IMessageSerializer Serializer
        {
            get => _serializer;
            set
            {
                _serializer = value;
                if (_serializer != null)
                    ContentType = _serializer.ContentType;
            }
        }

        SendContext<T> SendContext.CreateProxy<T>(T message)
        {
            return new SendContextProxy<T>(this, message);
        }

        public bool Durable { get; set; }

        public TMessage Message { get; }

        public Stream GetBodyStream()
        {
            return new MemoryStream(Body, false);
        }
    }
}