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
namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Pipeline;
    using RabbitMqTransport;
    using Scheduling;


    /// <summary>
    /// Extensions for scheduling publish/send message 
    /// </summary>
    public static class SchedulePublishExtensions
    {
        /// <summary>
        /// Sends a ScheduleMessage command to the endpoint, using the specified arguments
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="endpoint">The endpoint of the message scheduling service</param>
        /// <param name="host"></param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ISendEndpoint endpoint, IRabbitMqHost host, DateTime scheduledTime, T message)
            where T : class
        {
            var destinationAddress = GetDestinationAddress(host, typeof(T));

            return endpoint.ScheduleSend(destinationAddress, scheduledTime, message);
        }

        /// <summary>
        /// Sends a ScheduleMessage command to the endpoint, using the specified arguments
        /// </summary>
        /// <typeparam name="T">The scheduled message type</typeparam>
        /// <param name="endpoint">The endpoint of the message scheduling service</param>
        /// <param name="host"></param>
        /// <param name="scheduledTime">The time when the message should be sent to the endpoint</param>
        /// <param name="message">The message to send</param>
        /// <param name="sendPipe"></param>
        /// <returns>A handled to the scheduled message</returns>
        public static Task<ScheduledMessage<T>> SchedulePublish<T>(this ISendEndpoint endpoint, IRabbitMqHost host, DateTime scheduledTime, T message,
            IPipe<SendContext<ScheduleMessage<T>>> sendPipe)
            where T : class
        {
            var destinationAddress = GetDestinationAddress(host, typeof(T));

            return endpoint.ScheduleSend(destinationAddress, scheduledTime, message);
        }

        static Uri GetDestinationAddress(IRabbitMqHost host, Type messageType)
        {
            var sendSettings = host.GetSendSettings(messageType);

            return host.Settings.GetSendAddress(sendSettings);
        }
    }
}