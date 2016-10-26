// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Transport;


    public abstract class BaseClientSettings :
        ClientSettings
    {
        protected BaseClientSettings()
        {
            MaxConcurrentCalls = Math.Max(Environment.ProcessorCount, 8);
            PrefetchCount = Math.Max(MaxConcurrentCalls, 32);

            AutoRenewTimeout = TimeSpan.FromSeconds(60);
            MessageWaitTimeout = TimeSpan.FromDays(1);
        }

        public abstract TimeSpan AutoDeleteOnIdle { get; set; }

        public abstract TimeSpan DefaultMessageTimeToLive { get; set; }

        public abstract bool EnableBatchedOperations { set; }

        public abstract bool EnableDeadLetteringOnMessageExpiration { set; }

        public abstract string ForwardDeadLetteredMessagesTo { set; }

        public abstract int MaxDeliveryCount { get; set; }

        public abstract bool RequiresSession { get; set; }

        public abstract string UserMetadata { set; }

        public int MaxConcurrentCalls { get; set; }
        public int PrefetchCount { get; set; }
        public TimeSpan AutoRenewTimeout { get; set; }
        public TimeSpan MessageWaitTimeout { get; set; }

        public abstract TimeSpan LockDuration { get; set; }

        public abstract string Path { get; }

        public Uri GetInputAddress(Uri serviceUri)
        {
            var builder = new UriBuilder(serviceUri);

            builder.Path += Path;
            builder.Query += string.Join("&", GetQueryStringOptions());

            return builder.Uri;
        }

        protected abstract IEnumerable<string> GetQueryStringOptions();
    }
}