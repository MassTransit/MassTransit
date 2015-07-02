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
namespace MassTransit.Steward
{
    using System;
    using Messages;


    public static class ResourceUsageExtensions
    {
        public static void NotifyResourceUsageFailed(this ConsumeContext context, Uri resource, DateTime timestamp, TimeSpan duration, Exception exception)
        {
            Guid dispatchId = context.GetDispatchId();

            var @event = new ResourceUsageFailedEvent(dispatchId, resource, timestamp, duration, 500, exception.Message);

            context.Publish(@event);
        }

        public static void NotifyResourceUsageFailed(this ConsumeContext context, Uri resource, DateTime timestamp, TimeSpan duration, int reasonCode,
            string reasonText)
        {
            Guid dispatchId = context.GetDispatchId();

            var @event = new ResourceUsageFailedEvent(dispatchId, resource, timestamp, duration, reasonCode, reasonText);

            context.Publish(@event);
        }

        public static void NotifyResourceUsageCompleted(this ConsumeContext context, Uri resource, DateTime timestamp, TimeSpan duration)
        {
            Guid dispatchId = context.GetDispatchId();

            if (timestamp.Kind == DateTimeKind.Local)
                timestamp = timestamp.ToUniversalTime();

            var @event = new ResourceUsageCompletedEvent(resource, dispatchId, timestamp, duration);

            context.Publish(@event);
        }

        public static void NotifyResourceUsageCompleted(this ConsumeContext context, Guid dispatchId, Uri resource, DateTime timestamp, TimeSpan duration)
        {
            if (timestamp.Kind == DateTimeKind.Local)
                timestamp = timestamp.ToUniversalTime();

            var @event = new ResourceUsageCompletedEvent(resource, dispatchId, timestamp, duration);

            context.Publish(@event);
        }

        public static Guid GetDispatchId(this ConsumeContext context)
        {
            Guid? value = context.Headers.Get("X-MT-DispatchId", default(Guid?));
            if (value.HasValue)
                return value.Value;

            return Guid.Empty;
        }
    }
}