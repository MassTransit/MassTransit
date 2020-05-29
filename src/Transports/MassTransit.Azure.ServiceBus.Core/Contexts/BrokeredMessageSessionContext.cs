namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;


    public class BrokeredMessageSessionContext :
        MessageSessionContext
    {
        readonly IMessageSession _session;

        public BrokeredMessageSessionContext(IMessageSession session)
        {
            _session = session;
        }

        public Task<byte[]> GetStateAsync()
        {
            return _session.GetStateAsync();
        }

        public Task SetStateAsync(byte[] sessionState)
        {
            return _session.SetStateAsync(sessionState);
        }

        public Task RenewLockAsync(Message message)
        {
            return _session.RenewLockAsync(message);
        }

        public DateTime LockedUntilUtc => _session.LockedUntilUtc;

        public string SessionId => _session.SessionId;
    }
}
