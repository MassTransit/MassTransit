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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;


    abstract class CompletedExecutionResult<TArguments> :
        ExecutionResult
        where TArguments : class
    {
        readonly Activity _activity;
        readonly ExecuteContext<TArguments> _context;
        readonly IDictionary<string, object> _data;
        readonly TimeSpan _duration;
        readonly IRoutingSlipEventPublisher _publisher;
        readonly RoutingSlip _routingSlip;

        protected CompletedExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip)
            : this(context, publisher, activity, routingSlip, RoutingSlipBuilder.NoArguments)
        {
        }

        protected CompletedExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip,
            IDictionary<string, object> data)
        {
            _context = context;
            _publisher = publisher;
            _activity = activity;
            _routingSlip = routingSlip;
            _data = data;
            _duration = _context.Elapsed;
        }

        protected RoutingSlip RoutingSlip => _routingSlip;

        protected IRoutingSlipEventPublisher Publisher => _publisher;

        protected IDictionary<string, object> Data => _data;

        protected ExecuteContext<TArguments> Context => _context;

        protected Activity Activity => _activity;

        protected TimeSpan Duration => _duration;

        public async Task Evaluate()
        {
            RoutingSlipBuilder builder = CreateRoutingSlipBuilder(_routingSlip);

            Build(builder);

            RoutingSlip routingSlip = builder.Build();

            await PublishActivityEvents(routingSlip, builder).ConfigureAwait(false);

            if (HasNextActivity(routingSlip))
            {
                ISendEndpoint endpoint = await _context.GetSendEndpoint(routingSlip.GetNextExecuteAddress()).ConfigureAwait(false);

                await _context.Forward(endpoint, routingSlip).ConfigureAwait(false);
            }
            else
            {
                DateTime completedTimestamp = _context.Timestamp + _duration;
                TimeSpan completedDuration = completedTimestamp - _routingSlip.CreateTimestamp;

                await _publisher.PublishRoutingSlipCompleted(completedTimestamp, completedDuration, routingSlip.Variables).ConfigureAwait(false);
            }
        }

        public virtual bool IsFaulted(out Exception exception)
        {
            exception = default;
            return false;
        }

        protected virtual Task PublishActivityEvents(RoutingSlip routingSlip, RoutingSlipBuilder builder)
        {
            return Publisher.PublishRoutingSlipActivityCompleted(Context.ActivityName, Context.ExecutionId, Context.Timestamp, _duration,
                routingSlip.Variables, _activity.Arguments, _data);
        }

        static bool HasNextActivity(RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary.Any();
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
            builder.AddActivityLog(Context.Host, Activity.Name, Context.ExecutionId, Context.Timestamp, Duration);
        }

        protected virtual RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, routingSlip.Itinerary.Skip(1), Enumerable.Empty<Activity>());
        }
    }
}
