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
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Payloads;


    /// <summary>
    /// A consume context proxy creates a payload scope, such that anything added to the payload
    /// of the context is only added at the scope level and below.
    /// </summary>
    public abstract class ConsumeContextProxy :
        BaseConsumeContext
    {
        readonly ConsumeContext _context;

        protected ConsumeContextProxy(ConsumeContext context)
            : base(context)
        {
            _context = context;
        }

        protected ConsumeContextProxy(ConsumeContext context, IPayloadCache payloadCache)
            : base(context, payloadCache)
        {
            _context = context;
        }

        public override Guid? MessageId => _context.MessageId;
        public override Guid? RequestId => _context.RequestId;
        public override Guid? CorrelationId => _context.CorrelationId;
        public override Guid? ConversationId => _context.ConversationId;
        public override Guid? InitiatorId => _context.InitiatorId;
        public override DateTime? ExpirationTime => _context.ExpirationTime;
        public override Uri SourceAddress => _context.SourceAddress;
        public override Uri DestinationAddress => _context.DestinationAddress;
        public override Uri ResponseAddress => _context.ResponseAddress;
        public override Uri FaultAddress => _context.FaultAddress;
        public override DateTime? SentTime => _context.SentTime;

        public override Headers Headers => _context.Headers;
        public override HostInfo Host => _context.Host;

        public override Task ConsumeCompleted => _context.ConsumeCompleted;
        public override IEnumerable<string> SupportedMessageTypes => _context.SupportedMessageTypes;

        public override bool HasMessageType(Type messageType)
        {
            return _context.HasMessageType(messageType);
        }

        public override bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
        {
            if (_context.TryGetMessage(out ConsumeContext<T> messageContext))
            {
                consumeContext = new MessageConsumeContext<T>(this, messageContext.Message);
                return true;
            }

            consumeContext = null;
            return false;
        }

        public override Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            return _context.NotifyConsumed(context, duration, consumerType);
        }

        public override Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            return _context.NotifyFaulted(context, duration, consumerType, exception);
        }

        public override void AddConsumeTask(Task task)
        {
            _context.AddConsumeTask(task);
        }

        public override bool HasPayloadType(Type payloadType)
        {
            if (base.HasPayloadType(payloadType))
                return true;

            return _context.HasPayloadType(payloadType);
        }

        public override bool TryGetPayload<T>(out T payload)
        {
            if (base.TryGetPayload(out payload))
                return true;

            return _context.TryGetPayload(out payload);
        }

        public override T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
        {
            if (base.TryGetPayload<T>(out var existing))
                return existing;

            if (_context.TryGetPayload(out existing))
                return existing;

            return base.GetOrAddPayload(payloadFactory);
        }

        public override T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            if (base.TryGetPayload<T>(out var existing) || _context.TryGetPayload(out existing))
            {
                T Update(T _)
                {
                    return updateFactory(existing);
                }

                T Add()
                {
                    return updateFactory(existing);
                }

                return base.AddOrUpdatePayload(Add, Update);
            }

            return base.AddOrUpdatePayload(addFactory, updateFactory);
        }
    }


    /// <summary>
    /// A consume context proxy creates a payload scope, such that anything added to the payload
    /// of the context is only added at the scope level and below.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumeContextProxy<TMessage> :
        ConsumeContextProxy,
        ConsumeContext<TMessage>
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;

        protected ConsumeContextProxy(ConsumeContext<TMessage> context)
            : base(context)
        {
            _context = context;
        }

        public ConsumeContextProxy(ConsumeContext<TMessage> context, IPayloadCache payloadCache)
            : base(context, payloadCache)
        {
            _context = context;
        }

        public TMessage Message => _context.Message;

        public virtual Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return base.NotifyConsumed(this, duration, consumerType);
        }

        public virtual Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return base.NotifyFaulted(this, duration, consumerType, exception);
        }
    }
}