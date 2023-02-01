namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Used by the new outbox construct
    /// </summary>
    public interface OutboxSendContext :
        IServiceProvider
    {
        Task AddSend<T>(SendContext<T> context)
            where T : class;
    }
}
