namespace MassTransit.MessageData
{
    // not actually a true saga, but all the T-constraints are expecting it
    public class MessageDataSaga : ISaga
    {
        public Guid CorrelationId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Expires { get; set; }
        public Stream? Data { get; set; }
    }
}
