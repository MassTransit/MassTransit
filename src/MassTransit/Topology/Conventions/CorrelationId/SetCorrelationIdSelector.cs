namespace MassTransit.Topology.Conventions.CorrelationId
{
    using Context;


    public class SetCorrelationIdSelector<T> :
        ICorrelationIdSelector<T>
        where T : class
    {
        readonly ISetCorrelationId<T> _setCorrelationId;

        public SetCorrelationIdSelector(ISetCorrelationId<T> setCorrelationId)
        {
            _setCorrelationId = setCorrelationId;
        }

        public bool TryGetSetCorrelationId(out ISetCorrelationId<T> setCorrelationId)
        {
            setCorrelationId = _setCorrelationId;
            return true;
        }
    }
}
