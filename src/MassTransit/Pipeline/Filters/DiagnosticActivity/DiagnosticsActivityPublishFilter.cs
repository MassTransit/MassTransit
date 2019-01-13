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
    using Util;
    public class DiagnosticsActivityPublishFilter<T> :
        IFilter<PublishContext<T>> where T: class
    {
        const string ActivityIdHeader = "MT-Activity-ID";
        const string ActivityCorrelationContext = "MT-Activity-Correlation-Context";
        readonly DiagnosticSource _diagnosticSource;

        public DiagnosticsActivityPublishFilter(DiagnosticSource diagnosticSource)
        {
            _diagnosticSource = diagnosticSource;
        }

        public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            var messageType = TypeMetadataCache<T>.ShortName;
            var activity = StartIfEnabled(_diagnosticSource, $"Publishing Message {messageType}", new { context }, context);

            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            finally
            {
                StopIfEnabled(_diagnosticSource, activity, new { context });
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("send-activity-scope");
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

        static void StopIfEnabled(DiagnosticSource source, Activity activity, object args)
        {
            if (activity != null && source.IsEnabled(activity.OperationName))
                source.StopActivity(activity, args ?? new { });
        }
    }
}
#endif