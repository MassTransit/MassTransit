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
namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using InternalMessages;
    using Newtonsoft.Json;
    using Util;


    public class RoutingSlipEventPublisher :
        IRoutingSlipEventPublisher
    {
        readonly HostInfo _host;
        readonly IPublishEndpoint _publishEndpoint;
        readonly RoutingSlip _routingSlip;
        readonly ISendEndpointProvider _sendEndpointProvider;

        static RoutingSlipEventPublisher()
        {
            MessageCorrelation.UseCorrelationId<RoutingSlipCompleted>(x => x.TrackingNumber);
            MessageCorrelation.UseCorrelationId<RoutingSlipFaulted>(x => x.TrackingNumber);
            MessageCorrelation.UseCorrelationId<RoutingSlipActivityCompleted>(x => x.ExecutionId);
            MessageCorrelation.UseCorrelationId<RoutingSlipActivityFaulted>(x => x.ExecutionId);
            MessageCorrelation.UseCorrelationId<RoutingSlipActivityCompensated>(x => x.ExecutionId);
            MessageCorrelation.UseCorrelationId<RoutingSlipActivityCompensationFailed>(x => x.ExecutionId);
            MessageCorrelation.UseCorrelationId<RoutingSlipCompensationFailed>(x => x.TrackingNumber);
            MessageCorrelation.UseCorrelationId<RoutingSlipTerminated>(x => x.TrackingNumber);
            MessageCorrelation.UseCorrelationId<RoutingSlipRevised>(x => x.TrackingNumber);
        }

        public RoutingSlipEventPublisher(CompensateContext compensateContext, RoutingSlip routingSlip)
            : this(compensateContext, compensateContext, routingSlip)
        {
            _sendEndpointProvider = compensateContext;
            _publishEndpoint = compensateContext;
            _routingSlip = routingSlip;
            _host = compensateContext.Host;
        }

        public RoutingSlipEventPublisher(ExecuteContext executeContext, RoutingSlip routingSlip)
            : this(executeContext, executeContext, routingSlip)
        {
            _sendEndpointProvider = executeContext;
            _publishEndpoint = executeContext;
            _routingSlip = routingSlip;
            _host = executeContext.Host;
        }

        public RoutingSlipEventPublisher(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint, RoutingSlip routingSlip)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
            _routingSlip = routingSlip;
            _host = HostMetadataCache.Host;
        }

        public Task PublishRoutingSlipCompleted(DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables)
        {
            return PublishEvent<RoutingSlipCompleted>(RoutingSlipEvents.Completed, contents => new RoutingSlipCompletedMessage(
                _routingSlip.TrackingNumber,
                timestamp,
                duration,
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables))
                    ? variables
                    : GetEmptyObject()
                ));
        }

        public Task PublishRoutingSlipFaulted(DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables,
            params ActivityException[] exceptions)
        {
            return PublishEvent<RoutingSlipFaulted>(RoutingSlipEvents.Faulted, contents => new RoutingSlipFaultedMessage(
                _routingSlip.TrackingNumber,
                timestamp,
                duration,
                exceptions,
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables))
                    ? variables
                    : GetEmptyObject()
                ));
        }

        public Task PublishRoutingSlipActivityCompleted(string activityName, Guid executionId,
            DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, IDictionary<string, object> arguments,
            IDictionary<string, object> data)
        {
            return PublishEvent<RoutingSlipActivityCompleted>(RoutingSlipEvents.ActivityCompleted, contents => new RoutingSlipActivityCompletedMessage(
                _host,
                _routingSlip.TrackingNumber,
                activityName,
                executionId,
                timestamp,
                duration,
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables))
                    ? variables
                    : GetEmptyObject(),
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Arguments))
                    ? arguments
                    : GetEmptyObject(),
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Data))
                    ? data
                    : GetEmptyObject()));
        }

        public Task PublishRoutingSlipActivityFaulted(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, ExceptionInfo exceptionInfo,
            IDictionary<string, object> variables, IDictionary<string, object> arguments)
        {
            return PublishEvent<RoutingSlipActivityFaulted>(RoutingSlipEvents.ActivityFaulted, contents => new RoutingSlipActivityFaultedMessage(
                _host,
                _routingSlip.TrackingNumber,
                activityName,
                executionId,
                timestamp,
                duration,
                exceptionInfo,
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables))
                    ? variables
                    : GetEmptyObject(),
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Arguments))
                    ? arguments
                    : GetEmptyObject()));
        }

        public Task PublishRoutingSlipActivityCompensated(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration,
            IDictionary<string, object> variables, IDictionary<string, object> data)
        {
            return PublishEvent<RoutingSlipActivityCompensated>(RoutingSlipEvents.ActivityCompensated, contents => new RoutingSlipActivityCompensatedMessage(
                _host,
                _routingSlip.TrackingNumber,
                activityName,
                executionId,
                timestamp,
                duration,
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables))
                    ? variables
                    : GetEmptyObject(),
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Arguments))
                    ? data
                    : GetEmptyObject()));
        }

        public Task PublishRoutingSlipRevised(Guid executionId, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables,
            IList<Activity> itinerary, IList<Activity> previousItinerary)
        {
            return PublishEvent<RoutingSlipRevised>(RoutingSlipEvents.Revised, contents => new RoutingSlipRevisedMessage(
                _routingSlip.TrackingNumber,
                executionId,
                timestamp,
                duration,
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables))
                    ? variables
                    : GetEmptyObject(),
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Itinerary))
                    ? itinerary
                    : Enumerable.Empty<Activity>(),
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Itinerary))
                    ? previousItinerary
                    : Enumerable.Empty<Activity>()));
        }

        public Task PublishRoutingSlipTerminated(Guid executionId, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables,
            IList<Activity> previousItinerary)
        {
            return PublishEvent<RoutingSlipTerminated>(RoutingSlipEvents.Terminated, contents => new RoutingSlipTerminatedMessage(
                _routingSlip.TrackingNumber,
                executionId,
                timestamp,
                duration,
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables))
                    ? variables
                    : GetEmptyObject(),
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Itinerary))
                    ? previousItinerary
                    : Enumerable.Empty<Activity>()));
        }

        public Task PublishRoutingSlipActivityCompensationFailed(string activityName, Guid executionId,
            DateTime timestamp, TimeSpan duration, DateTime failureTimestamp, TimeSpan routingSlipDuration,
            ExceptionInfo exceptionInfo, IDictionary<string, object> variables, IDictionary<string, object> data)
        {
            return PublishEvent(RoutingSlipEvents.ActivityCompensationFailed | RoutingSlipEvents.CompensationFailed, contents => new CompensationFailed(
                _host,
                _routingSlip.TrackingNumber,
                activityName,
                executionId,
                timestamp,
                duration,
                failureTimestamp,
                routingSlipDuration,
                exceptionInfo,
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables))
                    ? variables
                    : GetEmptyObject(),
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Data))
                    ? data
                    : GetEmptyObject()));
        }

        static IDictionary<string, object> GetEmptyObject()
        {
            return JsonConvert.DeserializeObject<IDictionary<string, object>>("{}");
        }

        async Task PublishEvent<T>(RoutingSlipEvents eventFlag, Func<RoutingSlipEventContents, T> messageFactory)
            where T : class
        {
            if (_routingSlip.Subscriptions.Any())
            {
                foreach (var subscription in _routingSlip.Subscriptions)
                {
                    if (subscription.Events == RoutingSlipEvents.All || subscription.Events.HasFlag(eventFlag))
                    {
                        var endpoint = await _sendEndpointProvider.GetSendEndpoint(subscription.Address).ConfigureAwait(false);

                        var subscriptionMessage = messageFactory(subscription.Include);

                        await endpoint.Send(subscriptionMessage).ConfigureAwait(false);
                    }
                }
            }
            else
                await _publishEndpoint.Publish(messageFactory(RoutingSlipEventContents.All)).ConfigureAwait(false);
        }
    }
}