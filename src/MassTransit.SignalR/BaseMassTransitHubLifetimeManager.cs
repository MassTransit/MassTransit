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
    using MassTransit.SignalR.Contracts;
    using MassTransit.SignalR.Utils;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Protocol;
    using System;
    using System.Collections.Generic;

    public abstract class BaseMassTransitHubLifetimeManager<THub> : HubLifetimeManager<THub>
        where THub : Hub
    {
        protected readonly IRequestClient<GroupManagement<THub>> _groupManagementRequestClient;
        protected readonly IReadOnlyList<IHubProtocol> _protocols;

        protected readonly IPublishEndpoint _publishEndpoint;

        public BaseMassTransitHubLifetimeManager(
            IPublishEndpoint publishEndpoint,
            IClientFactory clientFactory,
            IHubProtocolResolver hubProtocolResolver)
        {
            _publishEndpoint = publishEndpoint;
            _groupManagementRequestClient = clientFactory.CreateRequestClient<GroupManagement<THub>>(TimeSpan.FromSeconds(20));
            _protocols = hubProtocolResolver.AllProtocols;
        }

        public string ServerName { get; } = GenerateServerName();

        public HubConnectionStore Connections { get; } = new HubConnectionStore();

        public MassTransitSubscriptionManager Groups { get; } = new MassTransitSubscriptionManager();
        public MassTransitSubscriptionManager Users { get; } = new MassTransitSubscriptionManager();

        static string GenerateServerName()
        {
            // Use the machine name for convenient diagnostics, but add a guid to make it unique.
            // Example: MyServerName_02db60e5fab243b890a847fa5c4dcb29
            return $"{Environment.MachineName}_{Guid.NewGuid():N}";
        }
    }
}
