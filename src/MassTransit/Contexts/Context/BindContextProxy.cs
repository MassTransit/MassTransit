namespace MassTransit.Context
{
    using System;
    using System.Reflection;
    using System.Threading;


    /// <summary>
    /// The BindContext
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    public class BindContextProxy<TLeft, TRight> :
        BindContext<TLeft, TRight>
        where TLeft : class, PipeContext
        where TRight : class
    {
        public BindContextProxy(TLeft left, TRight source)
        {
            Left = left;
            Right = source;
        }

        public TLeft Left { get; }

        public TRight Right { get; }

        public CancellationToken CancellationToken => Left.CancellationToken;

        public bool HasPayloadType(Type payloadType)
        {
            return payloadType.GetTypeInfo().IsInstanceOfType(Right) || Left.HasPayloadType(payloadType);
        }

        public bool TryGetPayload<T>(out T payload)
            where T : class
        {
            if (Right is T context)
            {
                payload = context;
                return true;
            }

            return Left.TryGetPayload(out payload);
        }

        public T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
            where T : class
        {
            if (Right is T context)
                return context;

            return Left.GetOrAddPayload(payloadFactory);
        }

        public T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
            where T : class
        {
            if (Right is T context)
                return context;

            return Left.AddOrUpdatePayload(addFactory, updateFactory);
        }
    }
}
