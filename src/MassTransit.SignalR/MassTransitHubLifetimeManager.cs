namespace MassTransit.SignalR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Logging;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Protocol;
    using Scoping;
    using Utils;


    public class MassTransitHubLifetimeManager<THub> :
        HubLifetimeManager<THub>
        where THub : Hub
    {
        readonly HubLifetimeManagerOptions<THub> _options;
        readonly IHubProtocolResolver _resolver;
        readonly IHubLifetimeScopeProvider _scopeProvider;

        public MassTransitHubLifetimeManager(HubLifetimeManagerOptions<THub> options, IHubLifetimeScopeProvider scopeProvider, IHubProtocolResolver resolver)
        {
            _options = options;
            _scopeProvider = scopeProvider;
            _resolver = resolver;
        }

        IReadOnlyList<IHubProtocol> Protocols => _resolver.AllProtocols;

        public string ServerName => _options.ServerName;
        public HubConnectionStore Connections => _options.ConnectionStore;
        public MassTransitSubscriptionManager Groups => _options.GroupsSubscriptionManager;
        public MassTransitSubscriptionManager Users => _options.UsersSubscriptionManager;

        public override Task OnConnectedAsync(HubConnectionContext connection)
        {
            var feature = new MassTransitFeature();
            connection.Features.Set<IMassTransitFeature>(feature);

            Connections.Add(connection);
            if (!string.IsNullOrEmpty(connection.UserIdentifier))
                Users.AddSubscription(connection.UserIdentifier, connection);

            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(HubConnectionContext connection)
        {
            Connections.Remove(connection);
            if (!string.IsNullOrEmpty(connection.UserIdentifier))
                Users.RemoveSubscription(connection.UserIdentifier, connection);

            // Also unsubscribe from any groups
            ConcurrentHashSet<string> groups = connection.Features.Get<IMassTransitFeature>().Groups;

            if (groups != null)
            {
                // Removes connection from all groups locally
                foreach (var groupName in groups.ToArray())
                    RemoveGroupAsyncCore(connection, groupName);
            }

            return Task.CompletedTask;
        }

        public override async Task SendAllAsync(string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            using IHubLifetimeScope<THub> scope = _scopeProvider.CreateScope<THub>();
            LogContext.Info?.Log("Publishing All<THub> message to MassTransit.");
            await scope.PublishEndpoint.Publish<All<THub>>(
                new {Messages = Protocols.ToProtocolDictionary(methodName, args)}, cancellationToken);
        }

        public override async Task SendAllExceptAsync(string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds,
            CancellationToken cancellationToken = default)
        {
            using IHubLifetimeScope<THub> scope = _scopeProvider.CreateScope<THub>();
            LogContext.Info?.Log("Publishing All<THub> message to MassTransit, with exceptions.");
            await scope.PublishEndpoint.Publish<All<THub>>(new
            {
                Messages = Protocols.ToProtocolDictionary(methodName, args),
                ExcludedConnectionIds = excludedConnectionIds.ToArray()
            }, cancellationToken);
        }

        public override async Task SendConnectionAsync(string connectionId, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            if (connectionId == null)
                throw new ArgumentNullException(nameof(connectionId));

            // If the connection is local we can skip sending the message through the bus since we require sticky connections.
            // This also saves serializing and deserializing the message!
            var connection = Connections[connectionId];
            if (connection != null)
            {
                // Connection is local, so we can skip publish
                await connection.WriteAsync(new InvocationMessage(methodName, args), cancellationToken).AsTask();
                return;
            }

            using IHubLifetimeScope<THub> scope = _scopeProvider.CreateScope<THub>();
            LogContext.Info?.Log("Publishing Connection<THub> message to MassTransit.");
            await scope.PublishEndpoint.Publish<Connection<THub>>(new
                {
                    ConnectionId = connectionId,
                    Messages = Protocols.ToProtocolDictionary(methodName, args)
                },
                cancellationToken);
        }

        public override async Task SendConnectionsAsync(IReadOnlyList<string> connectionIds, string methodName, object[] args,
            CancellationToken cancellationToken = default)
        {
            if (connectionIds == null)
                throw new ArgumentNullException(nameof(connectionIds));

            if (connectionIds.Any())
            {
                using IHubLifetimeScope<THub> scope = _scopeProvider.CreateScope<THub>();
                IReadOnlyDictionary<string, byte[]> protocolDictionary = Protocols.ToProtocolDictionary(methodName, args);
                IEnumerable<Task> publishTasks = connectionIds.Select(connectionId =>
                    scope.PublishEndpoint.Publish<Connection<THub>>(new
                    {
                        ConnectionId = connectionId,
                        Messages = protocolDictionary
                    }, cancellationToken));

                LogContext.Info?.Log("Publishing multiple Connection<THub> messages to MassTransit.");
                await Task.WhenAll(publishTasks);
            }
        }

        public override async Task SendGroupAsync(string groupName, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            using IHubLifetimeScope<THub> scope = _scopeProvider.CreateScope<THub>();
            LogContext.Info?.Log("Publishing Group<THub> message to MassTransit.");
            await scope.PublishEndpoint.Publish<Group<THub>>(new
                {
                    GroupName = groupName,
                    Messages = Protocols.ToProtocolDictionary(methodName, args)
                },
                cancellationToken);
        }

        public override async Task SendGroupExceptAsync(string groupName, string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds,
            CancellationToken cancellationToken = default)
        {
            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            using IHubLifetimeScope<THub> scope = _scopeProvider.CreateScope<THub>();
            LogContext.Info?.Log("Publishing Group<THub> message to MassTransit, with exceptions.");
            await scope.PublishEndpoint.Publish<Group<THub>>(new
            {
                GroupName = groupName,
                Messages = Protocols.ToProtocolDictionary(methodName, args),
                ExcludedConnectionIds = excludedConnectionIds.ToArray()
            }, cancellationToken);
        }

        public override async Task SendGroupsAsync(IReadOnlyList<string> groupNames, string methodName, object[] args,
            CancellationToken cancellationToken = default)
        {
            if (groupNames == null)
                throw new ArgumentNullException(nameof(groupNames));

            if (groupNames.Any())
            {
                using IHubLifetimeScope<THub> scope = _scopeProvider.CreateScope<THub>();
                IReadOnlyDictionary<string, byte[]> protocolDictionary = Protocols.ToProtocolDictionary(methodName, args);
                IEnumerable<Task> publishTasks = groupNames.Where(x => !string.IsNullOrEmpty(x)).Select(groupName =>
                    scope.PublishEndpoint.Publish<Group<THub>>(new
                    {
                        GroupName = groupName,
                        Messages = protocolDictionary
                    }, cancellationToken));

                LogContext.Info?.Log("Publishing multiple Group<THub> messages to MassTransit.");
                await Task.WhenAll(publishTasks);
            }
        }

        public override async Task SendUserAsync(string userId, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            using IHubLifetimeScope<THub> scope = _scopeProvider.CreateScope<THub>();
            LogContext.Info?.Log("Publishing User<THub> message to MassTransit.");
            await scope.PublishEndpoint.Publish<User<THub>>(new
            {
                UserId = userId,
                Messages = Protocols.ToProtocolDictionary(methodName, args)
            }, cancellationToken);
        }

        public override async Task SendUsersAsync(IReadOnlyList<string> userIds, string methodName, object[] args,
            CancellationToken cancellationToken = default)
        {
            if (userIds == null)
                throw new ArgumentNullException(nameof(userIds));

            if (userIds.Any())
            {
                using IHubLifetimeScope<THub> scope = _scopeProvider.CreateScope<THub>();
                IReadOnlyDictionary<string, byte[]> protocolDictionary = Protocols.ToProtocolDictionary(methodName, args);
                IEnumerable<Task> publishTasks = userIds.Select(userId => scope.PublishEndpoint.Publish<User<THub>>(new
                {
                    UserId = userId,
                    Messages = protocolDictionary
                }, cancellationToken));

                LogContext.Info?.Log("Publishing multiple User<THub> messages to MassTransit.");
                await Task.WhenAll(publishTasks);
            }
        }

        public override async Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            if (connectionId == null)
                throw new ArgumentNullException(nameof(connectionId));

            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            var connection = Connections[connectionId];
            if (connection != null)
            {
                // short circuit if connection is on this server
                AddGroupAsyncCore(connection, groupName);

                return;
            }

            // Publish to mass transit group management instead, but it waits for an ack...
            using IHubLifetimeScope<THub> scope = _scopeProvider.CreateScope<THub>();
            try
            {
                LogContext.Info?.Log("Publishing add GroupManagement<THub> message to MassTransit.");
                RequestHandle<GroupManagement<THub>> request = scope.RequestClient.Create(new
                    {
                        ConnectionId = connectionId,
                        GroupName = groupName,
                        ServerName,
                        Action = GroupAction.Add
                    },
                    cancellationToken);

                Response<Ack<THub>> ack = await request.GetResponse<Ack<THub>>();
                LogContext.Info?.Log($"Request Received for add GroupManagement<THub> from {ack.Message.ServerName}.");
            }
            catch (RequestTimeoutException e)
            {
                // That's okay, just log and swallow
                LogContext.Warning?.Log(e, "GroupManagement<THub> add ack timed out.", e);
            }
        }

        public override async Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            if (connectionId == null)
                throw new ArgumentNullException(nameof(connectionId));

            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            var connection = Connections[connectionId];
            if (connection != null)
            {
                // short circuit if connection is on this server
                RemoveGroupAsyncCore(connection, groupName);

                return;
            }

            // Publish to mass transit group management instead, but it waits for an ack...
            using IHubLifetimeScope<THub> scope = _scopeProvider.CreateScope<THub>();
            try
            {
                LogContext.Info?.Log("Publishing remove GroupManagement<THub> message to MassTransit.");
                RequestHandle<GroupManagement<THub>> request = scope.RequestClient.Create(new
                    {
                        ConnectionId = connectionId,
                        GroupName = groupName,
                        ServerName,
                        Action = GroupAction.Remove
                    },
                    cancellationToken);

                Response<Ack<THub>> ack = await request.GetResponse<Ack<THub>>();
                LogContext.Info?.Log($"Request Received for remove GroupManagement<THub> from {ack.Message.ServerName}.");
            }
            catch (RequestTimeoutException e)
            {
                // That's okay, just log and swallow
                LogContext.Warning?.Log(e, "GroupManagement<THub> remove ack timed out.", e);
            }
        }

        public void AddGroupAsyncCore(HubConnectionContext connection, string groupName)
        {
            var feature = connection.Features.Get<IMassTransitFeature>();
            feature.Groups.Add(groupName);

            Groups.AddSubscription(groupName, connection);
        }

        public void RemoveGroupAsyncCore(HubConnectionContext connection, string groupName)
        {
            Groups.RemoveSubscription(groupName, connection);

            var feature = connection.Features.Get<IMassTransitFeature>();
            feature.Groups.Remove(groupName);
        }
    }
}
