// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports
{
    using System;

    /// <summary>
    /// Tracks the inbound processing of messages by the endpoint. Once a message is 
    /// received successfully, it should be removed from the tracker. In the event a 
    /// message throws an exception, a retry count is tracked. Once the retry limit is exceeded,
    /// the message is moved to the error queue by the endpoint.
    /// </summary>
    public interface IInboundMessageTracker
    {
        /// <summary>
        /// Check if the message retry limit has been exceeded for the id specified.
        /// </summary>
        /// <param name="id">The message identifier</param>
        /// <param name="retryException">The exception to throw in association with the error queue</param>
        /// <returns>True if the message should no longer be processed and moved to the error queue</returns>
        bool IsRetryLimitExceeded(string id, out Exception retryException);

        /// <summary>
        /// Increment the retry count of the message as an exception has occurred.
        /// </summary>
        /// <param name="id">The message identifier</param>
        /// <param name="exception">The exception that was thrown by the consumer(s)</param>
        void IncrementRetryCount(string id, Exception exception);

        /// <summary>
        /// Marks the message as received successfully. This should remove the message tracking information
        /// from the tracker. It is suggested that for memory utilization reasons, an asynchronous timer based
        /// expiration also be used for messages that may be consumed by other processes.
        /// </summary>
        /// <param name="id">The message identifier</param>
        void MessageWasReceivedSuccessfully(string id);

        /// <summary>
        /// Marks the message as moved to the error queue.
        /// </summary>
        /// <param name="id">The message identifier</param>
        void MessageWasMovedToErrorQueue(string id);
    }
}