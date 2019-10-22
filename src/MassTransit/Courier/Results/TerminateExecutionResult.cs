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
namespace MassTransit.Courier.Results
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;


    class TerminateExecutionResult<TArguments> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
    {
        public TerminateExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity, RoutingSlip routingSlip)
            : base(context, publisher, activity, routingSlip)
        {
        }

        protected override RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, Enumerable.Empty<Activity>(), routingSlip.Itinerary.Skip(1));
        }

        protected override async Task PublishActivityEvents(RoutingSlip routingSlip, RoutingSlipBuilder builder)
        {
            await base.PublishActivityEvents(routingSlip, builder).ConfigureAwait(false);

            await Publisher.PublishRoutingSlipTerminated(Context.ActivityName, Context.ExecutionId, Context.Timestamp, Context.Elapsed, routingSlip.Variables,
                builder.SourceItinerary).ConfigureAwait(false);
        }
    }


    class TerminateWithVariablesExecutionResult<TArguments> :
        TerminateExecutionResult<TArguments>
        where TArguments : class
    {
        readonly IDictionary<string, object> _variables;

        public TerminateWithVariablesExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip, IDictionary<string, object> variables)
            : base(context, publisher, activity, routingSlip)
        {
            _variables = variables;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            base.Build(builder);

            builder.SetVariables(_variables);
        }
    }
}
