// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Filters.DiagnosticActivity
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using Util;


    public class DiagnosticsActivityConsumeFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        readonly string _activityCorrelationContext;
        readonly string _activityIdHeader;
        readonly DiagnosticSource _diagnosticSource;

        public DiagnosticsActivityConsumeFilter(DiagnosticSource diagnosticSource, string activityIdKey, string activityCorrelationContextKey)
        {
            _diagnosticSource = diagnosticSource;
            _activityIdHeader = activityIdKey;
            _activityCorrelationContext = activityCorrelationContextKey;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("consume-activity-scope");
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var activity = StartIfEnabled(_diagnosticSource, $"Consuming Message: {TypeMetadataCache<T>.ShortName}", new {context}, context);

            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            finally
            {
                StopIfEnabled(_diagnosticSource, activity, new {context});
            }
        }

        Activity StartIfEnabled(DiagnosticSource source, string name, object args, ConsumeContext context)
        {
            if (!source.IsEnabled(name) || context.TryGetPayload<Activity>(out _))
                return null;

            var activity = new Activity(name)
                .AddTag(DiagnosticHeaders.MessageId, context.MessageId.ToString())
                .AddTag(DiagnosticHeaders.InitiatorId, context.InitiatorId.ToString())
                .AddTag(DiagnosticHeaders.SourceAddress, context.SourceAddress.ToString())
                .AddTag(DiagnosticHeaders.DestinationAddress, context.DestinationAddress.ToString())
                .AddTag(DiagnosticHeaders.InputAddress, context.ReceiveContext.InputAddress.ToString())
                .AddTag(DiagnosticHeaders.SourceHostMachine, context.Host.MachineName)
                .AddTag(DiagnosticHeaders.SourceHostFrameworkVersion, context.Host.FrameworkVersion)
                .AddTag(DiagnosticHeaders.SourceHostProcessId, context.Host.ProcessId.ToString())
                .AddTag(DiagnosticHeaders.SourceHostMassTransitVersion, context.Host.MassTransitVersion)
                .AddTag(DiagnosticHeaders.MessageTypes, string.Join(",", context.SupportedMessageTypes))
                .AddBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId.ToString())
                .AddBaggage(DiagnosticHeaders.CorrelationConversationId, context.ConversationId.ToString());

            Extract(context, activity);

            source.StartActivity(activity, args ?? new { });

            return context.AddOrUpdatePayload(() => activity, a => activity);
        }

        void Extract(ConsumeContext context, Activity activity)
        {
            try
            {
                if (context.Headers.TryGetHeader(_activityIdHeader, out var parent) && !string.IsNullOrEmpty(parent?.ToString()))
                    activity.SetParentId(parent.ToString());

                foreach (KeyValuePair<string, string> item in context.Headers.Get(_activityCorrelationContext, Enumerable.Empty<KeyValuePair<string, string>>())
                )
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

        void StopIfEnabled(DiagnosticSource source, Activity activity, object args)
        {
            if (activity != null && source.IsEnabled(activity.OperationName))
                source.StopActivity(activity, args ?? new { });
        }
    }
}
#endif
