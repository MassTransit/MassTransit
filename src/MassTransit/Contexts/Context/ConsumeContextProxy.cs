namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A consume context proxy creates a payload scope, such that anything added to the payload
    /// of the context is only added at the scope level and below.
    /// </summary>
    public abstract class ConsumeContextProxy :
        BaseConsumeContext
    {
        readonly ConsumeContext _context;

        protected ConsumeContextProxy(ConsumeContext context)
            : base(context.ReceiveContext, context.SerializerContext)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the CancellationToken for the context (implicit interface)
        /// </summary>
        public override CancellationToken CancellationToken => _context.CancellationToken;

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

        public override void AddConsumeTask(Task task)
        {
            _context.AddConsumeTask(task);
        }

        /// <summary>
        /// Returns true if the payload type is included with or supported by the context type
        /// </summary>
        /// <param name="payloadType"></param>
        /// <returns></returns>
        public override bool HasPayloadType(Type payloadType)
        {
            return payloadType.IsInstanceOfType(this) || _context.HasPayloadType(payloadType);
        }

        /// <summary>
        /// Attempts to get the specified payload type
        /// </summary>
        /// <param name="payload"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override bool TryGetPayload<T>(out T payload)
        {
            if (this is T context)
            {
                payload = context;
                return true;
            }

            return _context.TryGetPayload(out payload);
        }

        /// <summary>
        /// Get or add a payload to the context, using the provided payload factory.
        /// </summary>
        /// <param name="payloadFactory">The payload factory, which is only invoked if the payload is not present.</param>
        /// <typeparam name="T">The payload type</typeparam>
        /// <returns></returns>
        public override T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
        {
            if (this is T context)
                return context;

            return _context.GetOrAddPayload(payloadFactory);
        }

        /// <summary>
        /// Either adds a new payload, or updates an existing payload
        /// </summary>
        /// <param name="addFactory">The payload factory called if the payload is not present</param>
        /// <param name="updateFactory">The payload factory called if the payload already exists</param>
        /// <typeparam name="T">The payload type</typeparam>
        /// <returns></returns>
        public override T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            if (this is T context)
                return context;

            return _context.AddOrUpdatePayload(addFactory, updateFactory);
        }

        public override Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            return _context.NotifyConsumed(context, duration, consumerType);
        }

        public override Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            return _context.NotifyFaulted(context, duration, consumerType, exception);
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

        public ConsumeContextProxy(ConsumeContext<TMessage> context)
            : base(context)
        {
            _context = context;
        }

        public TMessage Message => _context.Message;

        public virtual Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return NotifyConsumed(this, duration, consumerType);
        }

        public virtual Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyFaulted(this, duration, consumerType, exception);
        }
    }
}
