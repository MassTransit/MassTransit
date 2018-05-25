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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;


    public class ApplicationInsightsFilter<T> : IFilter<T>
        where T : class, PipeContext
    {
        const string MessageId = nameof(MessageId);
        const string ConversationId = nameof(ConversationId);
        const string CorrelationId = nameof(CorrelationId);

        const string StepName = "MassTransit:Consumer";

        readonly TelemetryClient _telemetryClient;

        public ApplicationInsightsFilter(TelemetryClient telemetryClient)
            => _telemetryClient = telemetryClient;

        public void Probe(ProbeContext context)
            => context.CreateFilterScope("ApplicationInsightsFilter");

        public async Task Send(T context, IPipe<T> next)
        {
            var consumeContext = context.GetPayload<ConsumeContext>();
            var messageType = consumeContext.SupportedMessageTypes.FirstOrDefault() ?? "Unknown";

            using (var operation = _telemetryClient.StartOperation<RequestTelemetry>($"{StepName} {messageType}"))
            {
                try
                {
                    operation.Telemetry.Properties.Add(MessageId, consumeContext.MessageId.ToString());
                    operation.Telemetry.Properties.Add(ConversationId, consumeContext.ConversationId.ToString());
                    operation.Telemetry.Properties.Add(CorrelationId, consumeContext.CorrelationId.ToString());

                    await next.Send(context).ConfigureAwait(false);

                    _telemetryClient.StopOperation(operation);
                }
                catch (Exception ex)
                {
                    var properties = new Dictionary<string, string>
                    {
                        {MessageId, consumeContext.MessageId.ToString()},
                        {ConversationId, consumeContext.ConversationId.ToString()},
                        {CorrelationId, consumeContext.CorrelationId.ToString()}
                    };

                    _telemetryClient.TrackException(ex, properties);

                    operation.Telemetry.Success = false;
                    throw;
                }
            }
        }
    }
}
