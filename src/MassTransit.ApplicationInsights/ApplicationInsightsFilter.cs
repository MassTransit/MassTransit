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


    public class ApplicationInsightsFilter<T> :
        IFilter<T>
        where T : class, ConsumeContext
    {
        const string MessageId = nameof(MessageId);
        const string ConversationId = nameof(ConversationId);
        const string CorrelationId = nameof(CorrelationId);
        const string RequestId = nameof(RequestId);

        const string StepName = "MassTransit:Consumer";

        readonly TelemetryClient _telemetryClient;

        public ApplicationInsightsFilter(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("ApplicationInsightsFilter");
        }

        public async Task Send(T context, IPipe<T> next)
        {
            var messageType = context.SupportedMessageTypes.FirstOrDefault() ?? "Unknown";

            using (IOperationHolder<RequestTelemetry> operation = _telemetryClient.StartOperation<RequestTelemetry>($"{StepName} {messageType}"))
            {
                if (context.MessageId.HasValue)
                    operation.Telemetry.Properties.Add(MessageId, context.MessageId.Value.ToString());

                if (context.ConversationId.HasValue)
                    operation.Telemetry.Properties.Add(ConversationId, context.ConversationId.Value.ToString());

                if (context.CorrelationId.HasValue)
                    operation.Telemetry.Properties.Add(CorrelationId, context.CorrelationId.Value.ToString());

                if (context.RequestId.HasValue)
                    operation.Telemetry.Properties.Add(RequestId, context.RequestId.Value.ToString());

                try
                {
                    await next.Send(context).ConfigureAwait(false);

                    _telemetryClient.StopOperation(operation);
                }
                catch (Exception ex)
                {
                    _telemetryClient.TrackException(ex, operation.Telemetry.Properties);

                    operation.Telemetry.Success = false;

                    throw;
                }
            }
        }
    }
}