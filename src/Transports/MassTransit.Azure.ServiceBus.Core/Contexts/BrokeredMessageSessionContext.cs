namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading.Tasks;
    using global::Azure.Messaging.ServiceBus;
    using MassTransit.Util;

    public class BrokeredMessageSessionContext :
        MessageSessionContext
    {
        readonly ProcessSessionMessageEventArgs _session;

        public BrokeredMessageSessionContext(ProcessSessionMessageEventArgs session)
        {
            _session = session;
        }

        public async Task<byte[]> GetStateAsync()
        {
            return (await _session.GetSessionStateAsync())?.ToArray();
        }

        public Task SetStateAsync(byte[] sessionState)
        {
            return _session.SetSessionStateAsync(new BinaryData(sessionState ?? Array.Empty<byte>()));
        }

        public Task RenewLockAsync(ServiceBusReceivedMessage message)
        {
            return TaskUtil.Completed;
        }

        public DateTime LockedUntilUtc => _session.Message.LockedUntil.UtcDateTime;

        public string SessionId => _session.SessionId;
    }
}
