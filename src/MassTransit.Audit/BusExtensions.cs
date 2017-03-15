// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Audit
{
    using System;
    using MassTransit;
    using Pipeline;


    public static class BusExtensions
    {
        /// <summary>
        /// Add audit for mesages that are sent or published.
        /// </summary>
        /// <param name="bus">The Bus</param>
        /// <param name="store">Audit store</param>
        /// <param name="configureFilter">Filter configuration delegate</param>
        /// <param name="metadataFactory">Message metadata factory. If opited, the default one will be used.</param>
        public static void UseSendAudit(this IBus bus, MessageAuditStore store,
            Action<IMessageFilterConfigurator> configureFilter = null,
            MessageAuditSendObserver.MetadataFactory metadataFactory = null)
        {
            var factory = metadataFactory ?? AuditMetadataFactories.DefaultSendContextMetadataFactory;
            bus.ConnectSendObserver(new MessageAuditSendObserver(store, factory, configureFilter));
            bus.ConnectPublishObserver(new MessageAuditPublishObserver(store, factory, configureFilter));
        }

        /// <summary>
        /// Add audit for mesages that are consumed.
        /// </summary>
        /// <param name="bus">The Bus</param>
        /// <param name="store">Audit store</param>
        /// <param name="configureFilter">Filter configuration delegate</param>
        /// <param name="metadataFactory">Message metadata factory. If opited, the default one will be used.</param>
        public static void UseConsumeAudit(this IConsumeObserverConnector bus, MessageAuditStore store,
            Action<IMessageFilterConfigurator> configureFilter = null,
            MessageAuditConsumeObserver.MetadataFactory metadataFactory = null)
        {
            bus.ConnectConsumeObserver(
                new MessageAuditConsumeObserver(store,
                    metadataFactory ?? AuditMetadataFactories.DefaultConsumeContextMetadataFactory,
                    configureFilter));
        }
    }
}