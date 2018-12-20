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
#if NETSTANDARD
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;


    public class DiagnosticsActivityFilter :
        IFilter<SendContext>,
        IFilter<ConsumeContext>
    {
        const string ActivityIdHeader = "MT-Activity-ID";
        const string ActivityCorrelationContext = "MT-Activity-Correlation-Context";
        readonly DiagnosticSource _diagnosticSource;

        public DiagnosticsActivityFilter(DiagnosticSource diagnosticSource)
        {
            _diagnosticSource = diagnosticSource;
        }

        public async Task Send(SendContext context, IPipe<SendContext> next)
        {
            var activity = StartIfEnabled(_diagnosticSource, "Sending Message", new {context}, context);

            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            finally
            {
                StopIfEnabled(_diagnosticSource, activity, new {context});
            }
        }

        public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            var activity = StartIfEnabled(_diagnosticSource, $"Consuming Message", new {context}, context);

            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            finally
            {
                StopIfEnabled(_diagnosticSource, activity, new {context});
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("activity-scope");
        }

        static Activity StartIfEnabled(DiagnosticSource source, string name, object args, SendContext context)
        {
            if (!source.IsEnabled(name) || context.TryGetPayload<Activity>(out _))
                return null;

            var activity = new Activity(name)
                .AddTag("message-id", context.MessageId.ToString())
                .AddTag("initiator-id", context.InitiatorId.ToString())
                .AddTag("source-address", context.SourceAddress.ToString())
                .AddTag("destination-address", context.DestinationAddress.ToString())
                .AddBaggage("correlation-id", context.CorrelationId.ToString())
                .AddBaggage("correlation-conversation-id", context.ConversationId.ToString());

            source.StartActivity(activity, args ?? new { });

            Inject(context, activity);

            return context.AddOrUpdatePayload(() => activity, a => activity);
        }

        static void Inject(SendContext context, Activity activity)
        {
            if (context.Headers.TryGetHeader(ActivityIdHeader, out _))
                return;

            context.Headers.Set(ActivityIdHeader, activity.Id);
            if (activity.Baggage.Any())
                context.Headers.Set(ActivityCorrelationContext, activity.Baggage.ToList());
        }

        static Activity StartIfEnabled(DiagnosticSource source, string name, object args, ConsumeContext context)
        {
            if (!source.IsEnabled(name) || context.TryGetPayload<Activity>(out _))
                return null;

            var activity = new Activity(name)
                .AddTag("message-id", context.MessageId.ToString())
                .AddTag("initiator-id", context.InitiatorId.ToString())
                .AddTag("source-address", context.SourceAddress.ToString())
                .AddTag("environment-host-machine", context.Host.MachineName)
                .AddTag("environment-host-framework-version", context.Host.FrameworkVersion)
                .AddTag("environment-host-process-id", context.Host.ProcessId.ToString())
                .AddTag("environment-host-mt-version", context.Host.MassTransitVersion)
                .AddTag("message-types", string.Join(",", context.SupportedMessageTypes))
                .AddBaggage("correlation-id", context.CorrelationId.ToString())
                .AddBaggage("correlation-conversation-id", context.ConversationId.ToString());

            Extract(context, activity);

            source.StartActivity(activity, args ?? new { });

            return context.AddOrUpdatePayload(() => activity, a => activity);
        }

        static void Extract(ConsumeContext context, Activity activity)
        {
            try
            {
                if (context.Headers.TryGetHeader(ActivityIdHeader, out var parent) && !string.IsNullOrEmpty(parent?.ToString()))
                    activity.SetParentId(parent.ToString());

                foreach (KeyValuePair<string, string> item in context.Headers.Get(ActivityCorrelationContext, Enumerable.Empty<KeyValuePair<string, string>>()))
                {
                    if (!string.IsNullOrEmpty(item.Value))
                        activity.AddBaggage(item.Key, item.Value);
                }
            }
            catch (Exception)
            {
                // ignored, if context is invalid, there nothing we can do:
                // invalid context was created by consumer, but if we throw here, it will break message processing on producer
                // and producer does not control which context it receives
            }
        }

        static void StopIfEnabled(DiagnosticSource source, Activity activity, object args)
        {
            if (activity != null && source.IsEnabled(activity.OperationName))
                source.StopActivity(activity, args ?? new { });
        }
    }
}
#endif