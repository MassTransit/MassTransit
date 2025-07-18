namespace MassTransit.Persistence
{
    [Serializable]
    public class SagaConcurrencyException :
        ConcurrencyException
    {
        public SagaConcurrencyException(string message, object instance)
            : base(message, instance.GetType(), (instance as ISaga)?.CorrelationId ?? Guid.Empty)
        {
        }

        public SagaConcurrencyException(string message, object instance, Exception innerException)
            : base(message, instance.GetType(), (instance as ISaga)?.CorrelationId ?? Guid.Empty, innerException)
        {
        }

        public SagaConcurrencyException()
        {
        }
    }
}
