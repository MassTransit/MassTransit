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
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using Util;


    public class DiagnosticsActivitySendFilter<T> :
        IFilter<SendContext<T>>
        where T : class
    {
        readonly string _activityCorrelationContext;
        readonly string _activityIdHeader;
        readonly DiagnosticSource _diagnosticSource;

        public DiagnosticsActivitySendFilter(DiagnosticSource diagnosticSource, string activityIdKey, string activityCorrelationContextKey)
        {
            _diagnosticSource = diagnosticSource;
            _activityIdHeader = activityIdKey;
            _activityCorrelationContext = activityCorrelationContextKey;
        }

        public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            var activity = StartIfEnabled(_diagnosticSource, $"Sending Message: {TypeMetadataCache<T>.ShortName}", new {context}, context);

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
            context.CreateFilterScope("send-activity-scope");
        }

        Activity StartIfEnabled(DiagnosticSource source, string name, object args, SendContext context)
        {
            if (!source.IsEnabled(name) || context.TryGetPayload<Activity>(out _))
                return null;

            var activity = new Activity(name)
                .AddTag(DiagnosticHeaders.MessageId, context.MessageId.ToString())
                .AddTag(DiagnosticHeaders.InitiatorId, context.InitiatorId.ToString())
                .AddTag(DiagnosticHeaders.SourceAddress, context.SourceAddress.ToString())
                .AddTag(DiagnosticHeaders.DestinationAddress, context.DestinationAddress.ToString())
                .AddBaggage(DiagnosticHeaders.CorrelationId, context.CorrelationId.ToString())
                .AddBaggage(DiagnosticHeaders.CorrelationConversationId, context.ConversationId.ToString());

            source.StartActivity(activity, args ?? new { });

            Inject(context, activity);

            return context.AddOrUpdatePayload(() => activity, a => activity);
        }

        void Inject(SendContext context, Activity activity)
        {
            if (context.Headers.TryGetHeader(_activityIdHeader, out _))
                return;

            context.Headers.Set(_activityIdHeader, activity.Id);
            if (activity.Baggage.Any())
                context.Headers.Set(_activityCorrelationContext, activity.Baggage.ToList());
        }

        void StopIfEnabled(DiagnosticSource source, Activity activity, object args)
        {
            if (activity != null && source.IsEnabled(activity.OperationName))
                source.StopActivity(activity, args ?? new { });
        }
    }
}
#endif
