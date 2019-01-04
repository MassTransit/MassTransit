// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ApplicationInsights
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;


    public class ApplicationInsightsConsumeFilter<T> : IFilter<T> where T : class, ConsumeContext
    {
        const string MessageId = nameof(MessageId);
        const string ConversationId = nameof(ConversationId);
        const string CorrelationId = nameof(CorrelationId);
        const string DestinationAddress = nameof(DestinationAddress);
        const string InputAddress = nameof(InputAddress);
        const string RequestId = nameof(RequestId);
        const string MessageType = nameof(MessageType);

        const string StepName = "MassTransit:Consumer";

        readonly TelemetryClient _telemetryClient;
        readonly Action<IOperationHolder<RequestTelemetry>, T> _configureOperation;
        readonly string _telemetryHeaderRootKey;
        readonly string _telemetryHeaderParentKey;

        public ApplicationInsightsConsumeFilter(
            TelemetryClient telemetryClient,
            Action<IOperationHolder<RequestTelemetry>, T> configureOperation,
            string telemetryHeaderRootKey,
            string telemetryHeaderParentKey)
        {
            _telemetryClient = telemetryClient;
            _configureOperation = configureOperation;
            _telemetryHeaderRootKey = telemetryHeaderRootKey;
            _telemetryHeaderParentKey = telemetryHeaderParentKey;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("TelemetryConsumeFilter");
        }

        public async Task Send(T context, IPipe<T> next)
        {
            var contextType = context.GetType();
            var messageType = contextType.GetGenericArguments().FirstOrDefault()?.FullName ?? "Unknown";

            // After the message is taken from the queue, create RequestTelemetry to track its processing.
            var requestTelemetry = new RequestTelemetry
            {
                Name = $"{StepName} {context.ReceiveContext.InputAddress.LocalPath} {messageType}",
            };

            requestTelemetry.Context.Operation.Id = context.Headers.Get<string>(_telemetryHeaderRootKey);
            requestTelemetry.Context.Operation.ParentId = context.Headers.Get<string>(_telemetryHeaderParentKey);

            using (IOperationHolder<RequestTelemetry> operation = _telemetryClient.StartOperation(requestTelemetry))
            {
                operation.Telemetry.Properties.Add(MessageType, messageType);

                if (context.MessageId.HasValue)
                    operation.Telemetry.Properties.Add(MessageId, context.MessageId.Value.ToString());

                if (context.ConversationId.HasValue)
                    operation.Telemetry.Properties.Add(ConversationId, context.ConversationId.Value.ToString());

                if (context.CorrelationId.HasValue)
                    operation.Telemetry.Properties.Add(CorrelationId, context.CorrelationId.Value.ToString());

                if (context.DestinationAddress != null)
                    operation.Telemetry.Properties.Add(DestinationAddress, context.DestinationAddress.ToString());

                if (context.ReceiveContext.InputAddress != null)
                    operation.Telemetry.Properties.Add(InputAddress, context.ReceiveContext.InputAddress.ToString());

                if (context.RequestId.HasValue)
                    operation.Telemetry.Properties.Add(RequestId, context.RequestId.Value.ToString());

                _configureOperation?.Invoke(operation, context);

                try
                {
                    await next.Send(context).ConfigureAwait(false);

                    operation.Telemetry.Success = true;
                }
                catch (Exception ex)
                {
                    _telemetryClient.TrackException(ex, operation.Telemetry.Properties);

                    operation.Telemetry.Success = false;
                    throw;
                }
                finally
                {
                    _telemetryClient.StopOperation(operation);
                }
            }
        }
    }
}