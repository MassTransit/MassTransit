namespace MassTransit.Monitoring.Health
{
    using System;
    using System.Threading.Tasks;
    using Attachments;
    using Util;


    public class BusAttachmentHealth :
        IBusAttachmentObserver
    {
        string _failureMessage = "not started";
        bool _healthy;

        public Task ConnectFaulted(Exception exception)
        {
            return Failure($"connect faulted: {exception.Message}");
        }

        public Task PreConnect(IBusAttachment busAttachment)
        {
            return TaskUtil.Completed;
        }

        public Task PostConnect(IBusAttachment busAttachment)
        {
            return Success();
        }

        public Task DisconnectFaulted(Exception exception)
        {
            return Failure($"disconnect faulted: {exception.Message}");
        }

        public Task PreDisconnect(IBusAttachment busAttachment)
        {
            return TaskUtil.Completed;
        }

        public Task PostDisconnect(IBusAttachment busAttachment)
        {
            return TaskUtil.Completed;
        }

        public HealthResult CheckHealth()
        {
            return _healthy
                ? HealthResult.Healthy("Ready")
                : HealthResult.Unhealthy($"Not ready: {_failureMessage}");
        }

        Task Failure(string message)
        {
            _healthy = false;
            _failureMessage = message;

            return TaskUtil.Completed;
        }

        Task Success()
        {
            _healthy = true;
            _failureMessage = "";

            return TaskUtil.Completed;
        }
    }
}
