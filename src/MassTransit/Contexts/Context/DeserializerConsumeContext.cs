namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public abstract class DeserializerConsumeContext :
        BaseConsumeContext
    {
        readonly PendingTaskCollection _consumeTasks;

        protected DeserializerConsumeContext(ReceiveContext receiveContext)
            : base(receiveContext)
        {
            _consumeTasks = new PendingTaskCollection(4);
        }

        protected DeserializerConsumeContext(ReceiveContext receiveContext, SerializerContext serializerContext)
            : base(receiveContext, serializerContext)
        {
            _consumeTasks = new PendingTaskCollection(4);
        }

        public override Task ConsumeCompleted => _consumeTasks.Completed(CancellationToken);

        /// <summary>
        /// Returns true if the payload type is included with or supported by the context type
        /// </summary>
        /// <param name="payloadType"></param>
        /// <returns></returns>
        public override bool HasPayloadType(Type payloadType)
        {
            return payloadType.IsInstanceOfType(this) || ReceiveContext.HasPayloadType(payloadType);
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

            return ReceiveContext.TryGetPayload(out payload);
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

            return ReceiveContext.GetOrAddPayload(payloadFactory);
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

            return ReceiveContext.AddOrUpdatePayload(addFactory, updateFactory);
        }

        public override void AddConsumeTask(Task task)
        {
            _consumeTasks.Add(task);
        }
    }
}
