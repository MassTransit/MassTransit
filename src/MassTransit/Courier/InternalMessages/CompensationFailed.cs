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
namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    public class CompensationFailed :
        RoutingSlipActivityCompensationFailed,
        RoutingSlipCompensationFailed
    {
        readonly TimeSpan _duration;
        readonly DateTime _failureTimestamp;
        readonly TimeSpan _routingSlipDuration;
        readonly DateTime _timestamp;

        public CompensationFailed(HostInfo host, Guid trackingNumber, string activityName, Guid activityTrackingNumber,
            DateTime timestamp, TimeSpan duration, DateTime failureTimestamp, TimeSpan routingSlipDuration,
            ExceptionInfo exceptionInfo, IDictionary<string, object> variables, IDictionary<string, object> data)
        {
            _failureTimestamp = failureTimestamp;
            _routingSlipDuration = routingSlipDuration;
            Host = host;
            _duration = duration;
            _timestamp = timestamp;

            TrackingNumber = trackingNumber;
            ActivityTrackingNumber = activityTrackingNumber;
            ActivityName = activityName;
            Data = data;
            Variables = variables;
            ExceptionInfo = exceptionInfo;
        }

        public Guid TrackingNumber { get; private set; }

        DateTime RoutingSlipActivityCompensationFailed.Timestamp
        {
            get { return _timestamp; }
        }

        public Guid ActivityTrackingNumber { get; private set; }
        public string ActivityName { get; private set; }
        public IDictionary<string, object> Data { get; private set; }
        public ExceptionInfo ExceptionInfo { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }

        TimeSpan RoutingSlipActivityCompensationFailed.Duration
        {
            get { return _duration; }
        }

        public HostInfo Host { get; private set; }

        DateTime RoutingSlipCompensationFailed.Timestamp
        {
            get { return _failureTimestamp; }
        }

        TimeSpan RoutingSlipCompensationFailed.Duration
        {
            get { return _routingSlipDuration; }
        }
    }
}