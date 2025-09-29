namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;


    public class ServiceBusMessageSessionContext :
        MessageSessionContext
    {
        readonly ProcessSessionMessageEventArgs _session;
        readonly CancellationToken _cancellationToken;

        public ServiceBusMessageSessionContext(ProcessSessionMessageEventArgs session, CancellationToken cancellationToken)
        {
            _session = session;
            _cancellationToken = cancellationToken;
        }

        public Task<BinaryData> GetStateAsync()
        {
            return _session.GetSessionStateAsync(_cancellationToken);
        }

        public Task SetStateAsync(BinaryData state)
        {
            return _session.SetSessionStateAsync(state, _cancellationToken);
        }

        public Task RenewLockAsync(ServiceBusReceivedMessage message)
        {
            return Task.CompletedTask;
        }

        public DateTime LockedUntilUtc => _session.Message.LockedUntil.UtcDateTime;

        public string SessionId => _session.SessionId;
    }
}
