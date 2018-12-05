namespace MassTransit.SignalR.Tests.OfficialFramework
{
    using Microsoft.AspNetCore.Connections;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Protocol;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class HubConnectionContextUtils
    {
        public static HubConnectionContext Create(ConnectionContext connection, IHubProtocol protocol = null, string userIdentifier = null)
        {
            return new HubConnectionContext(connection, TimeSpan.FromSeconds(15), NullLoggerFactory.Instance)
            {
                Protocol = protocol ?? new JsonHubProtocol(),
                UserIdentifier = userIdentifier,
            };
        }

        public static MockHubConnectionContext CreateMock(ConnectionContext connection)
        {
            return new MockHubConnectionContext(connection, TimeSpan.FromSeconds(15), NullLoggerFactory.Instance, TimeSpan.FromSeconds(15));
        }

        public class MockHubConnectionContext : HubConnectionContext
        {
            public MockHubConnectionContext(ConnectionContext connectionContext, TimeSpan keepAliveInterval, ILoggerFactory loggerFactory, TimeSpan clientTimeoutInterval)
                : base(connectionContext, keepAliveInterval, loggerFactory, clientTimeoutInterval)
            {

            }

            public override ValueTask WriteAsync(HubMessage message, CancellationToken cancellationToken = default)
            {
                throw new Exception();
            }
        }
    }
}
