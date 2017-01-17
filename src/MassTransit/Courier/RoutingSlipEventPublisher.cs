// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Contracts;
    using InternalMessages;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using Serialization;
    using Util;


    public class RoutingSlipEventPublisher :
        IRoutingSlipEventPublisher
    {
        readonly HostInfo _host;
        readonly IPublishEndpoint _publishEndpoint;
        readonly RoutingSlip _routingSlip;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly string _activityName;

        static RoutingSlipEventPublisher()
        {
            RoutingSlipEventCorrelation.ConfigureCorrelationIds();
        }

        public RoutingSlipEventPublisher(CompensateContext compensateContext, RoutingSlip routingSlip)
            : this(compensateContext, compensateContext, routingSlip)
        {
            _sendEndpointProvider = compensateContext;
            _publishEndpoint = compensateContext;
            _routingSlip = routingSlip;
            _activityName = compensateContext.ActivityName;
            _host = compensateContext.Host;
        }

        public RoutingSlipEventPublisher(ExecuteContext executeContext, RoutingSlip routingSlip)
            : this(executeContext, executeContext, routingSlip)
        {
            _sendEndpointProvider = executeContext;
            _publishEndpoint = executeContext;
            _routingSlip = routingSlip;
            _activityName = executeContext.ActivityName;
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
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables)
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
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables)
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
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables)
                    ? variables
                    : GetEmptyObject(),
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Arguments)
                    ? arguments
                    : GetEmptyObject(),
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Data)
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
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables)
                    ? variables
                    : GetEmptyObject(),
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Arguments)
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
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables)
                    ? variables
                    : GetEmptyObject(),
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Arguments)
                    ? data
                    : GetEmptyObject()));
        }

        public Task PublishRoutingSlipRevised(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration,
            IDictionary<string, object> variables,
            IList<Activity> itinerary, IList<Activity> previousItinerary)
        {
            return PublishEvent<RoutingSlipRevised>(RoutingSlipEvents.Revised, contents => new RoutingSlipRevisedMessage(
                _host,
                _routingSlip.TrackingNumber,
                activityName,
                executionId,
                timestamp,
                duration,
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables)
                    ? variables
                    : GetEmptyObject(),
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Itinerary)
                    ? itinerary
                    : Enumerable.Empty<Activity>(),
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Itinerary)
                    ? previousItinerary
                    : Enumerable.Empty<Activity>()));
        }

        public Task PublishRoutingSlipTerminated(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration,
            IDictionary<string, object> variables,
            IList<Activity> previousItinerary)
        {
            return PublishEvent<RoutingSlipTerminated>(RoutingSlipEvents.Terminated, contents => new RoutingSlipTerminatedMessage(
                _host,
                _routingSlip.TrackingNumber,
                activityName,
                executionId,
                timestamp,
                duration,
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables)
                    ? variables
                    : GetEmptyObject(),
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Itinerary)
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
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables)
                    ? variables
                    : GetEmptyObject(),
                contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Data)
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
            foreach (var subscription in _routingSlip.Subscriptions)
            {
                await PublishSubscriptionEvent(eventFlag, messageFactory, subscription).ConfigureAwait(false);
            }

            if (_routingSlip.Subscriptions.All(sub => sub.Events.HasFlag(RoutingSlipEvents.Supplemental)))
            {
                await _publishEndpoint.Publish(messageFactory(RoutingSlipEventContents.All)).ConfigureAwait(false);
            }
        }

        async Task PublishSubscriptionEvent<T>(RoutingSlipEvents eventFlag, Func<RoutingSlipEventContents, T> messageFactory, Subscription subscription)
            where T : class
        {
            if ((subscription.Events & RoutingSlipEvents.EventMask) == RoutingSlipEvents.All || subscription.Events.HasFlag(eventFlag))
            {
                if (string.IsNullOrWhiteSpace(_activityName) || string.IsNullOrWhiteSpace(subscription.ActivityName)
                    || _activityName.Equals(subscription.ActivityName, StringComparison.OrdinalIgnoreCase))
                {
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(subscription.Address).ConfigureAwait(false);

                var message = messageFactory(subscription.Include);

                if (subscription.Message != null)
                {
                    var adapter = new MessageEnvelopeContextAdapter(null, subscription.Message, JsonMessageSerializer.ContentTypeHeaderValue, message);

                    await endpoint.Send(message, adapter).ConfigureAwait(false);
                }
                else
                    await endpoint.Send(message).ConfigureAwait(false);
                }
            }
        }
    }
}