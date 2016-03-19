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
namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals.Extensions;
    using Pipeline;


    /// <summary>
    /// Converts the object type message to the appropriate generic type and invokes the send method with that
    /// generic overload.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageSchedulerConverter<T> :
        IMessageSchedulerConverter
        where T : class
    {
        public async Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var msg = message as T;
            if (msg == null)
                throw new ArgumentException("Unexpected message type: " + message.GetType().GetTypeName());

            ScheduledMessage scheduleSend = await scheduler.ScheduleSend(destinationAddress, scheduledTime, msg, cancellationToken).ConfigureAwait(false);

            return scheduleSend;
        }

        public async Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var msg = message as T;
            if (msg == null)
                throw new ArgumentException("Unexpected message type: " + message.GetType().GetTypeName());

            ScheduledMessage<T> scheduleSend =
                await scheduler.ScheduleSend(destinationAddress, scheduledTime, msg, pipe, cancellationToken).ConfigureAwait(false);

            return scheduleSend;
        }
    }
}