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
    using Contracts;


    class ActivityExceptionImpl :
        ActivityException
    {
        public ActivityExceptionImpl(string activityName, HostInfo host, Guid executionId, DateTime timestamp, TimeSpan elapsed,
            ExceptionInfo exceptionInfo)
        {
            ExecutionId = executionId;

            Timestamp = timestamp;
            Elapsed = elapsed;
            Name = activityName;
            Host = host;
            ExceptionInfo = exceptionInfo;
        }

        public Guid ExecutionId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Elapsed { get; private set; }
        public string Name { get; private set; }
        public HostInfo Host { get; private set; }
        public ExceptionInfo ExceptionInfo { get; private set; }
    }
}