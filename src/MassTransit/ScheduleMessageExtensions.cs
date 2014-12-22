// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pipeline;
    using Scheduling;


    /// <summary>
    /// Extensions for scheduling publish/send message 
    /// </summary>
    public static class ScheduleMessageExtensions
    {
        /// <summary>
        /// Sends a ScheduleMessage command to the endpoint, using the specified arguments
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="endpoint">The endpoint of the message scheduling service</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <returns>A handled to the scheduled message</returns>
        public static async Task<ScheduledMessage<T>> ScheduleSend<T>(this ISendEndpoint endpoint, Uri destinationAddress,
            DateTime scheduledTime, T message)
            where T : class
        {
            var command = new ScheduleMessageCommand<T>(scheduledTime, destinationAddress, message);

            await endpoint.Send<ScheduleMessage<T>>(command);

            return new ScheduledMessageHandle<T>(command.CorrelationId, command.ScheduledTime, command.Destination, command.Payload);
        }


        /// <summary>
        /// Cancel a scheduled message using the scheduled message instance
        /// </summary>
        /// <param name="endpoint">The endpoint of the scheduling service</param>
        /// <param name="message">The schedule message reference</param>
        public static Task CancelScheduledSend<T>(this ISendEndpoint endpoint, ScheduledMessage<T> message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");

            return CancelScheduledSend(endpoint, message.TokenId);
        }

        /// <summary>
        /// Cancel a scheduled message using the tokenId that was returned when the message was scheduled.
        /// </summary>
        /// <param name="endpoint">The endpoint of the scheduling service</param>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        public static async Task CancelScheduledSend(this ISendEndpoint endpoint, Guid tokenId)
        {
            var command = new CancelScheduledMessageCommand(tokenId);

            await endpoint.Send<CancelScheduledMessage>(command);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="publishEndpoint">The bus from which the scheduled message command should be published</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <param name="contextCallback">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static async Task<ScheduledMessage<T>> ScheduleMessage<T>(this IPublishEndpoint publishEndpoint, Uri destinationAddress, DateTime scheduledTime, T message,
            IPipe<PublishContext<ScheduleMessage<T>>> contextCallback = null)
            where T : class
        {
            var command = new ScheduleMessageCommand<T>(scheduledTime, destinationAddress, message);

            await publishEndpoint.Publish(command, contextCallback ?? Pipe.Empty<PublishContext<ScheduleMessage<T>>>());

            return new ScheduledMessageHandle<T>(command.CorrelationId, command.ScheduledTime, command.Destination,
                command.Payload);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="bus">The bus from which the scheduled message command should be published</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        ///  /// <param name="contextCallback">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> ScheduleMessage<T>(this IBus bus, DateTime scheduledTime, T message,
            IPipe<PublishContext<ScheduleMessage<T>>> contextCallback = null)
            where T : class
        {
            return ScheduleMessage(bus, bus.Address, scheduledTime, message, contextCallback);
        }

        /// <summary>
        /// Cancel a scheduled message using the scheduled message instance
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="message"> </param>
        public static Task CancelScheduledMessage<T>(this IPublishEndpoint bus, ScheduledMessage<T> message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");

            return CancelScheduledMessage(bus, message.TokenId);
        }

        /// <summary>
        /// Cancel a scheduled message using the tokenId that was returned when the message was scheduled.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        public static async Task CancelScheduledMessage(this IPublishEndpoint bus, Guid tokenId)
        {
            var command = new CancelScheduledMessageCommand(tokenId);

            await bus.Publish<CancelScheduledMessage>(command);
        }


        class CancelScheduledMessageCommand :
            CancelScheduledMessage
        {
            public CancelScheduledMessageCommand(Guid tokenId)
            {
                CorrelationId = NewId.NextGuid();
                Timestamp = DateTime.UtcNow;

                TokenId = tokenId;
            }

            public Guid TokenId { get; private set; }
            public DateTime Timestamp { get; private set; }
            public Guid CorrelationId { get; private set; }
        }


        class ScheduleMessageCommand<T> :
            ScheduleMessage<T>
            where T : class
        {
            public ScheduleMessageCommand(DateTime scheduledTime, Uri destination, T payload)
            {
                CorrelationId = NewId.NextGuid();

                ScheduledTime = scheduledTime.Kind == DateTimeKind.Local
                    ? scheduledTime.ToUniversalTime()
                    : scheduledTime;

                Destination = destination;
                Payload = payload;

                PayloadType = typeof(T).GetMessageTypes()
                    .Select(x => new MessageUrn(x).ToString())
                    .ToList();
            }

            public Guid CorrelationId { get; private set; }
            public DateTime ScheduledTime { get; private set; }
            public IList<string> PayloadType { get; private set; }
            public Uri Destination { get; private set; }
            public T Payload { get; private set; }
        }


        class ScheduledMessageHandle<T> :
            ScheduledMessage<T>
            where T : class
        {
            public ScheduledMessageHandle(Guid tokenId, DateTime scheduledTime, Uri destination, T payload)
            {
                TokenId = tokenId;
                ScheduledTime = scheduledTime;
                Destination = destination;
                Payload = payload;
            }

            public Guid TokenId { get; private set; }
            public DateTime ScheduledTime { get; private set; }
            public Uri Destination { get; private set; }
            public T Payload { get; private set; }
        }
    }
}