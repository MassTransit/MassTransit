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
namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// The context of a Message from AzureServiceBus - gives access to the transport
    /// message when requested.
    /// </summary>
    public interface BrokeredMessageContext
    {
        int DeliveryCount { get; }
        string Label { get; }
        long SequenceNumber { get; }
        long EnqueuedSequenceNumber { get; }
        string LockToken { get; }
        DateTime LockedUntil { get; }
        string SessionId { get; }
        long Size { get; }
        string To { get; }
        string ReplyToSessionId { get; }
        string PartitionKey { get; }
        string ViaPartitionKey { get; }
        string ReplyTo { get; }
        DateTime EnqueuedTime { get; }
        DateTime ScheduledEnqueueTime { get; }
        IDictionary<string, object> Properties { get; }
        TimeSpan TimeToLive { get; }
        string CorrelationId { get; }
        string MessageId { get; }
    }
}
