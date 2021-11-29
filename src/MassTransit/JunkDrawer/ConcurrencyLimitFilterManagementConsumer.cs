namespace MassTransit.JunkDrawer
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Middleware;


    /// <summary>
    /// Consumer which when connected to a management endpoint can control the concurrency
    /// limit.
    /// </summary>
    public class ConcurrencyLimitFilterManagementConsumer :
        IConsumer<SetConcurrencyLimit>
    {
        readonly string _id;
        readonly IPipeRouter _router;
        DateTime _lastUpdated;

        public ConcurrencyLimitFilterManagementConsumer(IPipeRouter router, string id = null)
        {
            _router = router;
            _id = id;

            _lastUpdated = DateTime.UtcNow;
        }

        public async Task Consume(ConsumeContext<SetConcurrencyLimit> context)
        {
            if (_id == null || _id.Equals(context.Message.Id, StringComparison.OrdinalIgnoreCase))
            {
                if (context.Message.Timestamp >= _lastUpdated)
                {
                    try
                    {
                        await _router.SetConcurrencyLimit(context.Message.ConcurrencyLimit).ConfigureAwait(false);

                        _lastUpdated = context.Message.Timestamp ?? context.SentTime ?? DateTime.UtcNow;

                        await context.RespondAsync<ConcurrencyLimitUpdated>(new
                        {
                            Timestamp = DateTime.UtcNow,
                            context.Message.Id,
                            context.Message.ConcurrencyLimit
                        }).ConfigureAwait(false);

                        LogContext.Debug?.Log("Set Consumer Limit: {ConcurrencyLimit} ({CommandId})", context.Message.ConcurrencyLimit, context.Message.Id);
                    }
                    catch (Exception exception)
                    {
                        LogContext.Error?.Log(exception, "Set Consumer Limit failed: {ConcurrencyLimit} ({CommandId})", context.Message.ConcurrencyLimit,
                            context.Message.Id);

                        throw;
                    }
                }
                else
                    throw new CommandException("The concurrency limit was updated after the command was sent.");
            }
        }
    }
}
