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
namespace MassTransit.Transports
{
    using System;
    using Logging;
    using Microsoft.Extensions.Logging;
    using Util;


    public static class ReceiveEndpointLoggingExtensions
    {
        static ILogger _logger = Logger.Get("MassTransit.Messages");

        public static void SetLog(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Log that a message was skipped, and moved to the dead letter queue
        /// </summary>
        /// <param name="context"></param>
        public static void LogSkipped(this ReceiveContext context)
        {
            _logger.LogDebug($"SKIP {context.InputAddress} {GetMessageId(context)}");
        }

        /// <summary>
        /// Log that a message was moved from one endpoint to the destination endpoint address
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destination"></param>
        /// <param name="reason"> </param>
        public static void LogMoved(this ReceiveContext context, string destination, string reason)
        {
            _logger.LogInformation($"MOVE {context.InputAddress} {GetMessageId(context)} {destination} {reason}");
        }

        static string GetMessageId(ReceiveContext context)
        {
            return context.TransportHeaders.Get("MessageId", "N/A");
        }

        public static void LogConsumed<T>(this ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            _logger.LogDebug(
                $"RECEIVE {context.ReceiveContext.InputAddress} {context.MessageId} {TypeMetadataCache<T>.ShortName} {consumerType}({duration})");
        }

        public static void LogFaulted<T>(this ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            var faultMessage = GetFaultMessage(exception);

            _logger.LogError(
                $"R-FAULT {context.ReceiveContext.InputAddress} {context.MessageId} {TypeMetadataCache<T>.ShortName} {consumerType}({duration}) {faultMessage}",
                exception);
        }

        public static void LogFaulted(this ReceiveContext context, Exception exception)
        {
            var faultMessage = GetFaultMessage(exception);

            _logger.LogError($"R-FAULT {context.InputAddress} {GetMessageId(context)} {faultMessage}", exception);
        }

        public static void LogRetry(this ConsumeContext context, Exception exception)
        {
            var faultMessage = GetFaultMessage(exception);

            _logger.LogWarning($"R-RETRY {context.ReceiveContext.InputAddress} {context.MessageId} {faultMessage}", exception);
        }

        public static void LogFaulted<T>(this SendContext<T> context, Exception exception)
            where T : class
        {
            var faultMessage = GetFaultMessage(exception);

            _logger.LogWarning($"S-FAULT {context.DestinationAddress} {context.MessageId} {TypeMetadataCache<T>.ShortName} {faultMessage}", exception);
        }

        public static void LogSent<T>(this SendContext<T> context)
            where T : class
        {
            _logger.LogDebug($"SEND {context.DestinationAddress} {context.MessageId} {TypeMetadataCache<T>.ShortName}");
        }

        public static void LogScheduled<T>(this SendContext<T> context, DateTime deliveryTime)
            where T : class
        {
            _logger.LogDebug(
                $"SCHED {context.DestinationAddress} {context.MessageId} {TypeMetadataCache<T>.ShortName} {deliveryTime:G} {context.ScheduledMessageId?.ToString("N") ?? ""}");
        }

        static string GetFaultMessage(Exception exception)
        {
            var baseException = exception.GetBaseException() ?? exception;

            return ExceptionUtil.GetMessage(baseException);
        }
    }
}
