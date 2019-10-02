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
    using System.Collections.Generic;
    using Context;
    using GreenPipes;
    using Metadata;
    using Util;


    public static class SendContextExtensions
    {
        public static void SetTextHeaders<T>(this IDictionary<string, T> dictionary, SendHeaders headers, Func<string, string, T> converter)
        {
            foreach (var header in headers.GetAll())
            {
                if (header.Value == null)
                {
                    if (dictionary.ContainsKey(header.Key))
                        dictionary.Remove(header.Key);

                    continue;
                }

                if (dictionary.ContainsKey(header.Key))
                    continue;

                if (header.Value is string stringValue)
                {
                    dictionary[header.Key] = converter(header.Key, stringValue);
                }
                else if (header.Value is IFormattable formatValue && formatValue.GetType().IsValueType)
                {
                    dictionary.Add(header.Key, converter(header.Key, formatValue.ToString()));
                }
            }
        }

        /// <summary>
        /// Set the host headers on the SendContext (for error, dead-letter, etc.)
        /// </summary>
        /// <param name="headers"></param>
        public static void SetHostHeaders(this SendHeaders headers)
        {
            headers.Set(MessageHeaders.Host.MachineName, HostMetadataCache.Host.MachineName);
            headers.Set(MessageHeaders.Host.ProcessName, HostMetadataCache.Host.ProcessName);
            headers.Set(MessageHeaders.Host.ProcessId, HostMetadataCache.Host.ProcessId.ToString("F0"));
            headers.Set(MessageHeaders.Host.Assembly, HostMetadataCache.Host.Assembly);
            headers.Set(MessageHeaders.Host.AssemblyVersion, HostMetadataCache.Host.AssemblyVersion);
            headers.Set(MessageHeaders.Host.MassTransitVersion, HostMetadataCache.Host.MassTransitVersion);
            headers.Set(MessageHeaders.Host.FrameworkVersion, HostMetadataCache.Host.FrameworkVersion);
            headers.Set(MessageHeaders.Host.OperatingSystemVersion, HostMetadataCache.Host.OperatingSystemVersion);
        }

        /// <summary>
        /// Set the host headers on the SendContext (for error, dead-letter, etc.)
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="exceptionContext"></param>
        public static void SetExceptionHeaders(this SendHeaders headers, ExceptionReceiveContext exceptionContext)
        {
            var exception = exceptionContext.Exception.GetBaseException() ?? exceptionContext.Exception;

            var exceptionMessage = ExceptionUtil.GetMessage(exception);

            headers.Set(MessageHeaders.Reason, "fault");

            headers.Set(MessageHeaders.FaultExceptionType, TypeMetadataCache.GetShortName(exception.GetType()));
            headers.Set(MessageHeaders.FaultMessage, exceptionMessage);
            headers.Set(MessageHeaders.FaultTimestamp, exceptionContext.ExceptionTimestamp.ToString("O"));
            headers.Set(MessageHeaders.FaultStackTrace, ExceptionUtil.GetStackTrace(exception));

            if (exceptionContext.TryGetPayload(out ConsumerFaultInfo info))
            {
                headers.Set(MessageHeaders.FaultConsumerType, info.ConsumerType);
                headers.Set(MessageHeaders.FaultMessageType, info.MessageType);
            }

            if (exceptionContext.TryGetPayload(out RetryContext retryContext) && retryContext.RetryCount > 0)
            {
                headers.Set(MessageHeaders.FaultRetryCount, retryContext.RetryCount);
            }
        }

        /// <summary>
        /// Transfer the header information from the ConsumeContext to the SendContext, including any non-MT headers.
        /// </summary>
        /// <param name="sendContext"></param>
        /// <param name="consumeContext"></param>
        public static void TransferConsumeContextHeaders(this SendContext sendContext, ConsumeContext consumeContext)
        {
            sendContext.AddOrUpdatePayload(() => consumeContext, _ => consumeContext);

            sendContext.SourceAddress = consumeContext.ReceiveContext.InputAddress;

            if (consumeContext.ConversationId.HasValue)
                sendContext.ConversationId = consumeContext.ConversationId;

            if (consumeContext.CorrelationId.HasValue)
                sendContext.InitiatorId = consumeContext.CorrelationId;
            else if (consumeContext.RequestId.HasValue)
                sendContext.InitiatorId = consumeContext.RequestId;

            foreach (KeyValuePair<string, object> header in consumeContext.Headers.GetAll())
            {
                if (header.Key.StartsWith("MT-"))
                    continue;

                // do not overwrite headers which have already been set
                if (sendContext.Headers.TryGetHeader(header.Key, out _))
                    continue;

                sendContext.Headers.Set(header.Key, header.Value);
            }
        }
    }
}
