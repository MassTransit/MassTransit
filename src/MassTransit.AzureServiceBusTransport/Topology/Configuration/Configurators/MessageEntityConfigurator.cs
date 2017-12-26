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
namespace MassTransit.AzureServiceBusTransport.Topology.Configuration.Configurators
{
    using System;
    using AzureServiceBusTransport.Configuration;


    public abstract class MessageEntityConfigurator :
        EntityConfigurator,
        IMessageEntityConfigurator
    {
        protected MessageEntityConfigurator(string path)
        {
            Path = path;

            AutoDeleteOnIdle = Defaults.AutoDeleteOnIdle;
            DefaultMessageTimeToLive = Defaults.DefaultMessageTimeToLive;
            EnableBatchedOperations = true;
        }

        public string Path { get; }

        public TimeSpan? DuplicateDetectionHistoryTimeWindow { get; set; }

        public bool? EnableExpress { get; set; }

        public bool? EnablePartitioning { get; set; }

        public bool? IsAnonymousAccessible { get; set; }

        public long? MaxSizeInMegabytes { get; set; }

        public bool? RequiresDuplicateDetection { get; set; }

        public bool? SupportOrdering { get; set; }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            RequiresDuplicateDetection = true;
            DuplicateDetectionHistoryTimeWindow = historyTimeWindow;
        }
    }
}