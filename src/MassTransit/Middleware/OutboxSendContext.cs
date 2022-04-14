namespace MassTransit.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// Used by the new outbox construct
    /// </summary>
    public interface OutboxSendContext
    {
        Task AddSend<T>(SendContext<T> context)
            where T : class;
    }
}
