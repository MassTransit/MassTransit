namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Linq;
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

            var data = filterContext.BackgroundJob.Job.Args.OfType<IHashedScheduleId>().FirstOrDefault();
            if (data == null || string.IsNullOrEmpty(data.HashId))
                return;

            try
            {
                using var transaction = filterContext.Connection.CreateWriteTransaction();
                transaction.RemoveHash(data.HashId);
                transaction.Commit();
            }
            catch (Exception e)
            {
                LogContext.Warning?.Log(e, "Hash: {HashId} could not been removed", data.HashId);
            }
        }
    }
}
