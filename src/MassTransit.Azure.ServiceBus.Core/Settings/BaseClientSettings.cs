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
namespace MassTransit.Azure.ServiceBus.Core.Settings
{
    using System;
    using System.Collections.Generic;
    using Topology.Configuration;
    using Transport;


    public abstract class BaseClientSettings :
        ClientSettings
    {
        protected BaseClientSettings(IEndpointEntityConfigurator configurator)
        {
            Configurator = configurator;

            MaxConcurrentCalls = Defaults.MaxConcurrentCalls;
            PrefetchCount = Defaults.PrefetchCount;
            MaxAutoRenewDuration = Defaults.MaxAutoRenewDuration;
            MessageWaitTimeout = Defaults.MessageWaitTimeout;
        }

        public IEndpointEntityConfigurator Configurator { get; }

        public bool UsingBasicTier { get; private set; }

        public int MaxConcurrentCalls { get; set; }
        public int PrefetchCount { get; set; }
        public TimeSpan MaxAutoRenewDuration { get; set; }
        public TimeSpan MessageWaitTimeout { get; set; }

        public abstract TimeSpan LockDuration { get; }
        public abstract bool RequiresSession { get; }

        public abstract string Path { get; }

        public string Name { get; set; }

        public Uri GetInputAddress(Uri serviceUri, string path)
        {
            var builder = new UriBuilder(serviceUri) {Path = path};

            builder.Query += string.Join("&", GetQueryStringOptions());

            return builder.Uri;
        }

        protected abstract IEnumerable<string> GetQueryStringOptions();

        public virtual void SelectBasicTier()
        {
            UsingBasicTier = true;
        }
    }
}