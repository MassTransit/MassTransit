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
namespace MassTransit
{
    public static class MessageHeaders
    {
        /// <summary>
        /// The reason for a message action being taken 
        /// </summary>
        public const string Reason = "MT-Reason";

        /// <summary>
        /// The type of exception from a Fault
        /// </summary>
        public const string FaultExceptionType = "MT-Fault-ExceptionType";

        /// <summary>
        /// The exception message from a Fault
        /// </summary>
        public const string FaultMessage = "MT-Fault-Message";

        /// <summary>
        /// The timestamp when the fault occurred
        /// </summary>
        public const string FaultTimestamp = "MT-Fault-Timestamp";

        /// <summary>
        /// The stack trace from a Fault
        /// </summary>
        public const string FaultStackTrace = "MT-Fault-StackTrace";

        /// <summary>
        /// The endpoint that forwarded the message to the new destination
        /// </summary>
        public const string ForwarderAddress = "MT-Forwarder-Address";

        /// <summary>
        /// The address where the message was originally delivered before being rescheduled
        /// </summary>
        public const string DeliveredAddress = "MT-Scheduling-DeliveredAddress";

        /// <summary>
        /// The tokenId for the message that was registered with the scheduler
        /// </summary>
        public const string SchedulingTokenId = "MT-Scheduling-TokenId";

        /// <summary>
        /// The number of times the message has been redelivered (zero if never)
        /// </summary>
        public const string RedeliveryCount = "MT-Redelivery-Count";

        /// <summary>
        /// The trigger key that was used when the scheduled message was trigger
        /// </summary>
        public const string QuartzTriggerKey = "MT-Quartz-TriggerKey";


        public static class Quartz
        {
            /// <summary>
            /// The time when the message was scheduled
            /// </summary>
            public const string Scheduled = "MT-Quartz-Scheduled";

            /// <summary>
            /// When the event for this message was fired by Quartz
            /// </summary>
            public const string Sent = "MT-Quartz-Sent";

            /// <summary>
            /// When the next message is scheduled to be sent
            /// </summary>
            public const string NextScheduled = "MT-Quartz-NextScheduled";

            /// <summary>
            /// When the previous message was sent
            /// </summary>
            public const string PreviousSent = "MT-Quartz-PreviousSent";
        }
    }
}