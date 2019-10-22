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
namespace MassTransit.MongoDbIntegration.Tests.Courier
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Courier.Contracts;
    using Metadata;
    using Util;


    class RoutingSlipActivityCompletedEvent :
        RoutingSlipActivityCompleted
    {
        public RoutingSlipActivityCompletedEvent(Guid trackingNumber, string activityName,
            Guid executionId, DateTime timestamp)
        {
            Timestamp = timestamp;
            TrackingNumber = trackingNumber;
            ActivityName = activityName;
            ExecutionId = executionId;

            Variables = new Dictionary<string, object>
            {
                {"Content", "Goodbye, cruel world."}
            };

            Data = new Dictionary<string, object>
            {
                {"OriginalContent", "Hello, World!"}
            };

            Arguments = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Data { get; private set; }

        public Guid TrackingNumber { get; private set; }

        public DateTime Timestamp { get; private set; }

        public TimeSpan Duration { get; private set; }

        public Guid ExecutionId { get; private set; }
        public string ActivityName { get; private set; }

        public HostInfo Host
        {
            get { return HostMetadataCache.Host; }
        }

        public IDictionary<string, object> Arguments { get; private set; }

        public IDictionary<string, object> Variables { get; private set; }
    }
}