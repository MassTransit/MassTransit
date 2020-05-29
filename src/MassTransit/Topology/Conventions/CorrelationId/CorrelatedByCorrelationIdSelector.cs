namespace MassTransit.Topology.Conventions.CorrelationId
{
    using System;
    using Context;
    using Context.SetCorrelationIds;
    using Internals.Extensions;


    public class CorrelatedByCorrelationIdSelector<T> :
        ICorrelationIdSelector<T>
        where T : class
    {
        public bool TryGetSetCorrelationId(out ISetCorrelationId<T> setCorrelationId)
        {
            var correlatedByInterface = typeof(T).GetInterface<CorrelatedBy<Guid>>();
            if (correlatedByInterface != null)
            {
                var objectType = typeof(CorrelatedBySetCorrelationId<>).MakeGenericType(typeof(T));
                setCorrelationId = (ISetCorrelationId<T>)Activator.CreateInstance(objectType);
                return true;
            }

            setCorrelationId = null;
            return false;
        }
    }
}
