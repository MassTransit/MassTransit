// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
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
    using MassTransit.MessageData;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Protocol;
    using Utils;


    public class MassTransitMessageDataHubLifetimeManager<THub> : BaseMassTransitHubLifetimeManager<THub>
        where THub : Hub
    {
        readonly IMessageDataRepository _repository;

        public MassTransitMessageDataHubLifetimeManager(IPublishEndpoint publishEndpoint,
            IClientFactory clientFactory,
            IHubProtocolResolver hubProtocolResolver,
            IMessageDataRepository repository)
            : base(publishEndpoint, clientFactory, hubProtocolResolver)
        {
            _repository = repository;
        }

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

            // Also unsubscrube from any groups
            ConcurrentHashSet<string> groups = connection.Features.Get<IMassTransitFeature>().Groups;

            if (groups != null)
                groups.Clear(); // Removes connection from all groups locally

            return Task.CompletedTask;
        }

        public override async Task SendAllAsync(string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            LogContext.Info?.Log("Publishing All<THub> message to MassTransit.");
            await _publishEndpoint.Publish<AllMessageData<THub>>(
                new {Messages = await _protocols.ToProtocolDictionary(methodName, args).ToMessageData(_repository)}, cancellationToken);
        }

        public override async Task SendAllExceptAsync(string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds,
            CancellationToken cancellationToken = default)
        {
            LogContext.Info?.Log("Publishing All<THub> message to MassTransit, with exceptions.");
            await _publishEndpoint.Publish<AllMessageData<THub>>(
                new
                {
                    Messages = await _protocols.ToProtocolDictionary(methodName, args).ToMessageData(_repository),
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
                await connection.WriteAsync(new InvocationMessage(methodName, args)).AsTask();
                return;
            }

            LogContext.Info?.Log("Publishing Connection<THub> message to MassTransit.");
            await _publishEndpoint.Publish<ConnectionMessageData<THub>>(new
                {
                    ConnectionId = connectionId,
                    Messages = await _protocols.ToProtocolDictionary(methodName, args).ToMessageData(_repository)
                },
                cancellationToken);
        }

        public override async Task SendConnectionsAsync(IReadOnlyList<string> connectionIds, string methodName, object[] args,
            CancellationToken cancellationToken = default)
        {
            if (connectionIds == null)
                throw new ArgumentNullException(nameof(connectionIds));

            if (connectionIds.Count > 0)
            {
                var protocolDictionary = _protocols.ToProtocolDictionary(methodName, args);
                var publishTasks = new List<Task>(connectionIds.Count);

                foreach (var connectionId in connectionIds)
                    publishTasks.Add(_publishEndpoint.Publish<Connection<THub>>(new
                        {
                            ConnectionId = connectionId,
                            Messages = await protocolDictionary.ToMessageData(_repository)
                        },
                        cancellationToken));

                LogContext.Info?.Log("Publishing multiple Connection<THub> messages to MassTransit.");
                await Task.WhenAll(publishTasks);
            }
        }

        public override async Task SendGroupAsync(string groupName, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            LogContext.Info?.Log("Publishing Group<THub> message to MassTransit.");
            await _publishEndpoint.Publish<GroupMessageData<THub>>(new
                {
                    GroupName = groupName,
                    Messages = await _protocols.ToProtocolDictionary(methodName, args).ToMessageData(_repository)
                },
                cancellationToken);
        }

        public override async Task SendGroupExceptAsync(string groupName, string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds,
            CancellationToken cancellationToken = default)
        {
            if (groupName == null)
                throw new ArgumentNullException(nameof(groupName));

            LogContext.Info?.Log("Publishing Group<THub> message to MassTransit, with exceptions.");
            await _publishEndpoint.Publish<GroupMessageData<THub>>(
                new
                {
                    GroupName = groupName,
                    Messages = await _protocols.ToProtocolDictionary(methodName, args).ToMessageData(_repository),
                    ExcludedConnectionIds = excludedConnectionIds.ToArray()
                }, cancellationToken);
        }

        public override async Task SendGroupsAsync(IReadOnlyList<string> groupNames, string methodName, object[] args,
            CancellationToken cancellationToken = default)
        {
            if (groupNames == null)
                throw new ArgumentNullException(nameof(groupNames));

            if (groupNames.Count > 0)
            {
                var protocolDictionary = _protocols.ToProtocolDictionary(methodName, args);
                var publishTasks = new List<Task>(groupNames.Count);

                foreach (var groupName in groupNames)
                {
                    if (!string.IsNullOrEmpty(groupName))
                        publishTasks.Add(_publishEndpoint.Publish<GroupMessageData<THub>>(new
                        {
                            GroupName = groupName,
                            Messages = await protocolDictionary.ToMessageData(_repository)
                        }, cancellationToken));
                }

                LogContext.Info?.Log("Publishing multiple Group<THub> messages to MassTransit.");
                await Task.WhenAll(publishTasks);
            }
        }

        public override async Task SendUserAsync(string userId, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            LogContext.Info?.Log("Publishing User<THub> message to MassTransit.");
            await _publishEndpoint.Publish<UserMessageData<THub>>(new
            {
                UserId = userId,
                Messages = await _protocols.ToProtocolDictionary(methodName, args).ToMessageData(_repository)
            }, cancellationToken);
        }

        public override async Task SendUsersAsync(IReadOnlyList<string> userIds, string methodName, object[] args,
            CancellationToken cancellationToken = default)
        {
            if (userIds == null)
                throw new ArgumentNullException(nameof(userIds));

            if (userIds.Count > 0)
            {
                var protocolDictionary = _protocols.ToProtocolDictionary(methodName, args);
                var publishTasks = new List<Task>(userIds.Count);

                foreach (var userId in userIds)
                    publishTasks.Add(_publishEndpoint.Publish<UserMessageData<THub>>(new
                    {
                        UserId = userId,
                        Messages = await protocolDictionary.ToMessageData(_repository)
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
            try
            {
                LogContext.Info?.Log("Publishing add GroupManagement<THub> message to MassTransit.");
                RequestHandle<GroupManagement<THub>> request =
                    _groupManagementRequestClient.Create(new
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
            try
            {
                LogContext.Info?.Log("Publishing remove GroupManagement<THub> message to MassTransit.");
                RequestHandle<GroupManagement<THub>> request =
                    _groupManagementRequestClient.Create(new
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
