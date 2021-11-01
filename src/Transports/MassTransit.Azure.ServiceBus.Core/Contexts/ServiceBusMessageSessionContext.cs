namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading.Tasks;
    using global::Azure.Messaging.ServiceBus;
    using Util;


    public class ServiceBusMessageSessionContext :
        MessageSessionContext
    {
        readonly ProcessSessionMessageEventArgs _session;

        public ServiceBusMessageSessionContext(ProcessSessionMessageEventArgs session)
        {
            _session = session;
        }

        public Task<BinaryData> GetStateAsync()
        {
            return _session.GetSessionStateAsync();
        }

        public Task SetStateAsync(BinaryData state)
        {
            return _session.SetSessionStateAsync(state);
        }

        public Task RenewLockAsync(ServiceBusReceivedMessage message)
        {
            return TaskUtil.Completed;
        }

        public DateTime LockedUntilUtc => _session.Message.LockedUntil.UtcDateTime;

        public string SessionId => _session.SessionId;
    }
}
