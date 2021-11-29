namespace MassTransit.Configuration
{
    public class SetCorrelationIdSelector<T> :
        ICorrelationIdSelector<T>
        where T : class
    {
        readonly IMessageCorrelationId<T> _messageCorrelationId;

        public SetCorrelationIdSelector(IMessageCorrelationId<T> messageCorrelationId)
        {
            _messageCorrelationId = messageCorrelationId;
        }

        public bool TryGetSetCorrelationId(out IMessageCorrelationId<T> messageCorrelationId)
        {
            messageCorrelationId = _messageCorrelationId;
            return true;
        }
    }
}
