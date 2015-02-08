// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceBus.Messaging;


    /// <summary>
    /// The context of a BrokeredMessage from AzureServiceBus - gives access to the transport
    /// message when requested.
    /// </summary>
    public interface BrokeredMessageContext
    {
        int DeliveryCount { get; }
        string Label { get; }
        long SequenceNumber { get; }
        long EnqueuedSequenceNumber { get; }
        Guid LockToken { get; }
        DateTime LockedUntil { get; }
        string SessionId { get; }
        long Size { get; }
        MessageState State { get; }
        bool ForcePersistence { get; }
        string To { get; }
        string ReplyToSessionId { get; }
        string PartitionKey { get; }
        string ViaPartitionKey { get; }
        string ReplyTo { get; }
        DateTime EnqueuedTime { get; }
        DateTime ScheduledEnqueueTime { get; }
        IDictionary<string, object> Properties { get; }
    }
}