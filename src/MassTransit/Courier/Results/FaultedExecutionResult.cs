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
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Events;
    using InternalMessages;


    class FaultedExecutionResult<TArguments> :
        ExecutionResult
        where TArguments : class
    {
        readonly Activity _activity;
        readonly ActivityException _activityException;
        readonly TimeSpan _elapsed;
        readonly ExceptionInfo _exceptionInfo;
        readonly ExecuteContext<TArguments> _executeContext;
        readonly IRoutingSlipEventPublisher _publisher;
        readonly RoutingSlip _routingSlip;
        readonly Exception _exception;

        public FaultedExecutionResult(ExecuteContext<TArguments> executeContext, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip, Exception exception)
        {
            _executeContext = executeContext;
            _publisher = publisher;
            _activity = activity;
            _routingSlip = routingSlip;
            _exception = exception;
            _exceptionInfo = new FaultExceptionInfo(exception);
            _elapsed = _executeContext.Elapsed;

            _activityException = new ActivityExceptionImpl(_activity.Name, _executeContext.Host, _executeContext.ExecutionId,
                _executeContext.Timestamp, _elapsed, _exceptionInfo);
        }

        public async Task Evaluate()
        {
            await _publisher.PublishRoutingSlipActivityFaulted(_executeContext.ActivityName, _executeContext.ExecutionId, _executeContext.Timestamp,
                _elapsed, _exceptionInfo, _routingSlip.Variables, _activity.Arguments).ConfigureAwait(false);

            if (HasCompensationLogs())
            {
                RoutingSlipBuilder builder = CreateRoutingSlipBuilder(_routingSlip);

                Build(builder);

                RoutingSlip routingSlip = builder.Build();

                await _executeContext.Forward(routingSlip.GetNextCompensateAddress(), routingSlip).ConfigureAwait(false);
            }
            else
            {
                DateTime faultedTimestamp = _executeContext.Timestamp + _elapsed;
                TimeSpan faultedDuration = faultedTimestamp - _routingSlip.CreateTimestamp;

                await _publisher.PublishRoutingSlipFaulted(faultedTimestamp, faultedDuration, _routingSlip.Variables,
                    _activityException).ConfigureAwait(false);
            }
        }

        public bool IsFaulted(out Exception exception)
        {
            exception = _exception;
            return true;
        }

        bool HasCompensationLogs()
        {
            return _routingSlip.CompensateLogs != null && _routingSlip.CompensateLogs.Count > 0;
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
            builder.AddActivityException(_activityException);
        }

        protected virtual RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, routingSlip.Itinerary, Enumerable.Empty<Activity>());
        }
    }
}
