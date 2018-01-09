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


    public static class ServiceBusSendContextExtensions
    {
        /// <summary>
        /// Set the time at which the message should be delivered to the queue
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheduledTime">The scheduled time for the message</param>
        public static void SetScheduledEnqueueTime(this SendContext context, DateTime scheduledTime)
        {
            if (context.TryGetPayload(out ServiceBusSendContext sendContext))
            {
                if (scheduledTime.Kind == DateTimeKind.Local)
                    scheduledTime = scheduledTime.ToUniversalTime();

                sendContext.ScheduledEnqueueTimeUtc = scheduledTime;
            }
        }

        /// <summary>
        /// Set the time at which the message should be delivered to the queue
        /// </summary>
        /// <param name="context"></param>
        /// <param name="delay">The time to wait before the message shuould be enqueued</param>
        public static void SetScheduledEnqueueTime(this SendContext context, TimeSpan delay)
        {
            if (context.TryGetPayload(out ServiceBusSendContext sendContext))
            {
                sendContext.ScheduledEnqueueTimeUtc = DateTime.UtcNow + delay;
            }
        }

        public static void SetSessionId(this SendContext context, string sessionId)
        {
            if (context.TryGetPayload(out ServiceBusSendContext sendContext))
            {
                sendContext.SessionId = sessionId;
            }
        }

        public static void SetReplyToSessionId(this SendContext context, string sessionId)
        {
            if (context.TryGetPayload(out ServiceBusSendContext sendContext))
            {
                sendContext.ReplyToSessionId = sessionId;
            }
        }
        public static void SetPartitionKey(this SendContext context, string partitionKey)
        {
            if (context.TryGetPayload(out ServiceBusSendContext sendContext))
            {
                sendContext.PartitionKey = partitionKey;
            }
        }
    }
}