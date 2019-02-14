// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Settings
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceBus.Messaging;
    using Topology.Configuration;
    using Topology.Configuration.Configurators;
    using Transport;


    public class ReceiveEndpointSettings :
        BaseClientSettings,
        ReceiveSettings
    {
        readonly QueueConfigurator _queueConfigurator;

        public ReceiveEndpointSettings(string queueName, QueueConfigurator queueConfigurator)
            : base(queueConfigurator)
        {
            _queueConfigurator = queueConfigurator;

            Name = queueName;
        }

        public IQueueConfigurator QueueConfigurator => _queueConfigurator;

        public bool RemoveSubscriptions { get; set; }

        public override TimeSpan LockDuration => _queueConfigurator.LockDuration ?? Defaults.LockDuration;

        public override string Path => _queueConfigurator.FullPath;

        public override bool RequiresSession => _queueConfigurator.RequiresSession ?? false;

        public QueueDescription GetQueueDescription()
        {
            return _queueConfigurator.GetQueueDescription();
        }

        public override void SelectBasicTier()
        {
            _queueConfigurator.AutoDeleteOnIdle = default;
            _queueConfigurator.EnableExpress = default;
            _queueConfigurator.DefaultMessageTimeToLive = Defaults.BasicMessageTimeToLive;
        }

        protected override IEnumerable<string> GetQueryStringOptions()
        {
            if (_queueConfigurator.EnableExpress.HasValue && _queueConfigurator.EnableExpress.Value)
                yield return "express=true";

            if (_queueConfigurator.AutoDeleteOnIdle.HasValue && _queueConfigurator.AutoDeleteOnIdle.Value > TimeSpan.Zero
                && _queueConfigurator.AutoDeleteOnIdle.Value != Defaults.AutoDeleteOnIdle)
                yield return $"autodelete={_queueConfigurator.AutoDeleteOnIdle.Value.TotalSeconds}";
        }
    }
}