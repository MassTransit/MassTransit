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
    using System.Collections.Generic;
    using Contracts;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Protocol;
    using Utils;


    public abstract class BaseMassTransitHubLifetimeManager<THub> :
        HubLifetimeManager<THub>
        where THub : Hub
    {
        protected readonly IRequestClient<GroupManagement<THub>> GroupManagementRequestClient;
        protected readonly IReadOnlyList<IHubProtocol> Protocols;
        readonly HubLifetimeManagerOptions<THub> _hubLifetimeManagerOptions;
        protected readonly IPublishEndpoint PublishEndpoint;

        protected BaseMassTransitHubLifetimeManager(HubLifetimeManagerOptions<THub> hubLifetimeManagerOptions,
            IPublishEndpoint publishEndpoint,
            IRequestClient<GroupManagement<THub>> groupManagementRequestClient,
            IHubProtocolResolver hubProtocolResolver)
        {
            _hubLifetimeManagerOptions = hubLifetimeManagerOptions;
            PublishEndpoint = publishEndpoint;
            GroupManagementRequestClient = groupManagementRequestClient;
            Protocols = hubProtocolResolver.AllProtocols;
        }

        public string ServerName => _hubLifetimeManagerOptions.ServerName;
        public HubConnectionStore Connections => _hubLifetimeManagerOptions.ConnectionStore;
        public MassTransitSubscriptionManager Groups => _hubLifetimeManagerOptions.GroupsSubscriptionManager;
        public MassTransitSubscriptionManager Users => _hubLifetimeManagerOptions.UsersSubscriptionManager;
    }
}
