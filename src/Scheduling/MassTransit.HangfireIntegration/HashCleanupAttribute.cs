namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Linq;
    using Context;
    using Hangfire.Common;
    using Hangfire.Server;


    public class HashCleanupAttribute :
        JobFilterAttribute,
        IServerFilter
    {
        public void OnPerforming(PerformingContext filterContext)
        {
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            if (filterContext.Exception != null && !filterContext.ExceptionHandled)
                return;

            var data = filterContext.BackgroundJob.Job.Args.OfType<IHashable>().FirstOrDefault();
            if (data == null || string.IsNullOrEmpty(data.HashId))
                return;

            try
            {
                using var transaction = filterContext.Connection.CreateWriteTransaction();
                transaction.RemoveHash(data.HashId);
                transaction.Commit();
                LogContext.Debug?.Log("Hash: {HashId} has been removed", data.HashId);
            }
            catch (Exception e)
            {
                LogContext.Warning?.Log(e, "Hash: {HashId} could not been removed", data.HashId);
            }
        }
    }
}
