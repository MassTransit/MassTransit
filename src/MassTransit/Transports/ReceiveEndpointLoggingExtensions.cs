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
    using Util;


    public static class ReceiveEndpointLoggingExtensions
    {
        static readonly ILog _messages = Logger.Get("MassTransit.Messages");

        /// <summary>
        /// Log that a message was skipped, and moved to the dead letter queue
        /// </summary>
        /// <param name="context"></param>
        public static void LogSkipped(this ReceiveContext context)
        {
            if (_messages.IsDebugEnabled)
                _messages.Debug($"SKIP {context.InputAddress} {GetMessageId(context)}");
        }

        /// <summary>
        /// Log that a message was moved from one endpoint to the destination endpoint address
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="reason"> </param>
        public static void LogMoved(this ReceiveContext context, Uri destinationAddress, string reason)
        {
            if (_messages.IsInfoEnabled)
                _messages.Info($"MOVE {context.InputAddress} {GetMessageId(context)} {destinationAddress} {reason}");
        }

        static string GetMessageId(ReceiveContext context)
        {
            return context.TransportHeaders.Get("MessageId", "N/A");
        }

        public static void LogConsumed<T>(this ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            if (_messages.IsDebugEnabled)
                _messages.Debug(
                    $"RECEIVE {context.ReceiveContext.InputAddress} {GetMessageId(context.ReceiveContext)} {TypeMetadataCache<T>.ShortName} {consumerType}({duration})");
        }

        public static void LogFaulted<T>(this ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            if (_messages.IsErrorEnabled)
            {
                var faultMessage = GetFaultMessage(exception);

                _messages.Error(
                    $"R-FAULT {context.ReceiveContext.InputAddress} {GetMessageId(context.ReceiveContext)} {TypeMetadataCache<T>.ShortName} {consumerType}({duration}) {faultMessage}",
                    exception);
            }
        }

        public static void LogRetry(this ConsumeContext context, Exception exception)
        {
            if (_messages.IsWarnEnabled)
            {
                var faultMessage = GetFaultMessage(exception);

                _messages.Warn($"R-RETRY {context.ReceiveContext.InputAddress} {GetMessageId(context.ReceiveContext)} {faultMessage}", exception);
            }
        }

        public static void LogFaulted<T>(this SendContext<T> context, Exception exception)
            where T : class
        {
            if (_messages.IsWarnEnabled)
            {
                var faultMessage = GetFaultMessage(exception);

                _messages.Warn($"S-FAULT {context.DestinationAddress} {context.MessageId} {TypeMetadataCache<T>.ShortName} {faultMessage}", exception);
            }
        }

        public static void LogSent<T>(this SendContext<T> context)
            where T : class
        {
            if (_messages.IsDebugEnabled)
                _messages.Debug($"SEND {context.DestinationAddress} {context.MessageId} {TypeMetadataCache<T>.ShortName}");
        }

        public static void LogScheduled<T>(this SendContext<T> context, DateTime deliveryTime)
            where T : class
        {
            if (_messages.IsDebugEnabled)
                _messages.Debug(
                    $"SCHED {context.DestinationAddress} {context.MessageId} {TypeMetadataCache<T>.ShortName} {deliveryTime:G} {context.ScheduledMessageId?.ToString("N") ?? ""}");
        }

        static string GetFaultMessage(Exception exception)
        {
            var baseException = exception.GetBaseException() ?? exception;

            return ExceptionUtil.GetMessage(baseException);
        }
    }
}