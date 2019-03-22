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
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class MassTransitDependencyInjectionExtensions
    {
        /// <summary>
        /// Registers the Hub Lifetime Manager based on the configuration options provided
        /// </summary>
        /// <param name="signalRServerBuilder">The SignalR builder abstraction for configuring SignalR servers.</param>
        /// <param name="configureOptions">The MassTransit SignalR configuration options</param>
        public static void AddMassTransitBackplane(this ISignalRServerBuilder signalRServerBuilder,
            Action<MassTransitSignalROptions> configureOptions = null)
        {
            var options = new MassTransitSignalROptions();

            configureOptions?.Invoke(options);

            signalRServerBuilder.Services.AddSingleton(options);

            if (options.UseMessageData)
                signalRServerBuilder.Services.AddSingleton(typeof(HubLifetimeManager<>), typeof(MassTransitMessageDataHubLifetimeManager<>));
            else
                signalRServerBuilder.Services.AddSingleton(typeof(HubLifetimeManager<>), typeof(MassTransitHubLifetimeManager<>));
        }
    }
}
