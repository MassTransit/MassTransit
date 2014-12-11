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
namespace MassTransit.Scheduling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


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
        public static ScheduledMessage<T> ScheduleSend<T>(this IEndpoint endpoint, Uri destinationAddress, DateTime scheduledTime, T message)
            where T : class
        {
            var command = new ScheduleMessageCommand<T>(scheduledTime, destinationAddress, message);

            endpoint.Send<ScheduleMessage<T>>(command);

            return new ScheduledMessageHandle<T>(command.CorrelationId, command.ScheduledTime, command.Destination,
                command.Payload);
        }

        /// <summary>
        /// Sends a ScheduleMessage command to the endpoint, using the specified arguments
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="endpoint">The endpoint of the message scheduling service</param>
        /// <param name="destinationAddress">The destionation address for the schedule message</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        public static ScheduledMessage<T> ScheduleSend<T>(this IEndpoint endpoint, IEndpointAddress destinationAddress, DateTime scheduledTime, T message)
            where T : class
        {
            return ScheduleSend(endpoint, destinationAddress.Uri, scheduledTime, message);
        }

        /// <summary>
        /// Sends a ScheduleMessage command to the endpoint, using the specified arguments
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="endpoint">The endpoint of the message scheduling service</param>
        /// <param name="bus">The bus instance where the scheduled message should be sent</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        public static ScheduledMessage<T> ScheduleSend<T>(this IEndpoint endpoint, IServiceBus bus, DateTime scheduledTime, T message)
            where T : class
        {
            return ScheduleSend(endpoint, bus.Endpoint.Address.Uri, scheduledTime, message);
        }

        /// <summary>
        /// Sends a ScheduleMessage command to the endpoint, using the specified arguments
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="endpoint">The endpoint of the message scheduling service</param>
        /// <param name="destinationEndpoint">The destination endpoint where the scheduled message should be sent</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        public static ScheduledMessage<T> ScheduleSend<T>(this IEndpoint endpoint, IEndpoint destinationEndpoint, DateTime scheduledTime, T message)
            where T : class
        {
            return ScheduleSend(endpoint, destinationEndpoint.Address.Uri, scheduledTime, message);
        }

        /// <summary>
        /// Cancel a scheduled message using the scheduled message instance
        /// </summary>
        /// <param name="endpoint">The endpoint of the scheduling service</param>
        /// <param name="message">The schedule message reference</param>
        public static void CancelScheduledSend<T>(this IEndpoint endpoint, ScheduledMessage<T> message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");

            CancelScheduledSend(endpoint, message.TokenId);
        }

        /// <summary>
        /// Cancel a scheduled message using the tokenId that was returned when the message was scheduled.
        /// </summary>
        /// <param name="endpoint">The endpoint of the scheduling service</param>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        public static void CancelScheduledSend(this IEndpoint endpoint, Guid tokenId)
        {
            var command = new CancelScheduledMessageCommand(tokenId);

            endpoint.Send<CancelScheduledMessage>(command);
        }


        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="bus">The bus from which the scheduled message command should be published</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <param name="contextCallback">Optional: A callback that gives the caller access to the publish context.</param>
        /// <returns>A handled to the scheduled message</returns>
        public static ScheduledMessage<T> ScheduleMessage<T>(this IServiceBus bus, Uri destinationAddress, DateTime scheduledTime, T message, Action<IPublishContext<ScheduleMessage<T>>> contextCallback = null)
            where T : class
        {
            var command = new ScheduleMessageCommand<T>(scheduledTime, destinationAddress, message);

            bus.Publish<ScheduleMessage<T>>(command, contextCallback ?? (c => {}));

            return new ScheduledMessageHandle<T>(command.CorrelationId, command.ScheduledTime, command.Destination,
                command.Payload);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="bus">The bus from which the scheduled message command should be published</param>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <returns>A handled to the scheduled message</returns>
        public static ScheduledMessage<T> ScheduleMessage<T>(this IServiceBus bus, IEndpointAddress destinationAddress, DateTime scheduledTime, T message)
            where T : class
        {
            return ScheduleMessage(bus, destinationAddress.Uri, scheduledTime, message);
        }

        /// <summary>
        /// Schedules a message to be sent to the bus using a Publish, which should only be used when
        /// the quartz service is on a single shared queue or behind a distributor
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="bus">The bus from which the scheduled message command should be published</param>
        /// <param name="destinationEndpoint">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <returns>A handled to the scheduled message</returns>
        public static ScheduledMessage<T> ScheduleMessage<T>(this IServiceBus bus, IEndpoint destinationEndpoint, DateTime scheduledTime, T message)
            where T : class
        {
            return ScheduleMessage(bus, destinationEndpoint.Address.Uri, scheduledTime, message);
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
        public static ScheduledMessage<T> ScheduleMessage<T>(this IServiceBus bus, DateTime scheduledTime, T message, Action<IPublishContext<ScheduleMessage<T>>> contextCallback = null)
            where T : class
        {
            return ScheduleMessage(bus, bus.Endpoint.Address.Uri, scheduledTime, message, contextCallback);
        }

        /// <summary>
        /// Cancel a scheduled message using the scheduled message instance
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="message"> </param>
        public static void CancelScheduledMessage<T>(this IServiceBus bus, ScheduledMessage<T> message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException("message");

            CancelScheduledMessage(bus, message.TokenId);
        }

        /// <summary>
        /// Cancel a scheduled message using the tokenId that was returned when the message was scheduled.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        public static void CancelScheduledMessage(this IServiceBus bus, Guid tokenId)
        {
            var command = new CancelScheduledMessageCommand(tokenId);

            bus.Publish<CancelScheduledMessage>(command);
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