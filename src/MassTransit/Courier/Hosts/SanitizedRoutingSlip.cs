// Copyright 2007-2014 Chris Patterson
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
namespace MassTransit.Courier.Hosts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Contracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serialization;


    public class SanitizedRoutingSlip :
        RoutingSlip
    {
        readonly JToken _messageToken;
        readonly JToken _variablesToken;

        public SanitizedRoutingSlip(ConsumeContext<RoutingSlip> context)
        {
            ConsumeContext<JToken> messageTokenContext;
            if (!context.TryGetMessage(out messageTokenContext))
                throw new InvalidOperationException("Unable to retrieve JSON token");

            _messageToken = messageTokenContext.Message;

            RoutingSlip routingSlip = context.Message;

            TrackingNumber = routingSlip.TrackingNumber;

            _variablesToken = _messageToken["variables"] ?? new JObject();

            Variables = routingSlip.Variables ?? GetEmptyObject();

            Itinerary = (routingSlip.Itinerary ?? Enumerable.Empty<Activity>())
                .Select(x => (Activity)new SanitizedActivity(x))
                .ToList();

            ActivityLogs = (routingSlip.ActivityLogs ?? Enumerable.Empty<ActivityLog>())
                .Select(x => (ActivityLog)new SanitizedActivityLog(x))
                .ToList();

            CompensateLogs = (routingSlip.CompensateLogs ?? Enumerable.Empty<CompensateLog>())
                .Select(x => (CompensateLog)new SanitizedCompensateLog(x))
                .ToList();

            ActivityExceptions = (routingSlip.ActivityExceptions ?? Enumerable.Empty<ActivityException>())
                .Select(x => (ActivityException)new SanitizedActivityException(x))
                .ToList();

            Subscriptions = (routingSlip.Subscriptions ?? Enumerable.Empty<Subscription>())
                .Select(x => (Subscription)new SanitizedSubscription(x))
                .ToList();
        }


        public Guid TrackingNumber { get; private set; }
        public DateTime CreateTimestamp { get; private set; }
        public IList<Activity> Itinerary { get; private set; }
        public IList<ActivityLog> ActivityLogs { get; private set; }
        public IList<CompensateLog> CompensateLogs { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
        public IList<ActivityException> ActivityExceptions { get; private set; }
        public IList<Subscription> Subscriptions { get; private set; }


        public T GetActivityArguments<T>()
            where T : class
        {
            return GetActivityArguments(new DefaultJsonTypeConverter<T>());
        }

        public T GetActivityArguments<T>(JsonTypeConverter<T> converter)
            where T : class
        {
            JToken itineraryToken = _messageToken["itinerary"];
            if (itineraryToken == null)
                throw new ArgumentException("Itinerary not found in the routing slip");

            JToken activityToken = itineraryToken is JArray ? itineraryToken[0] : itineraryToken;
            if (activityToken == null)
                throw new ArgumentException("Activity not found in the routing slip");

            JToken token = _variablesToken.Merge(activityToken["arguments"]);

            return converter.Convert(token);
        }

        public T GetCompensateLogData<T>()
        {
            JToken activityLogsToken = _messageToken["compensateLogs"];

            JToken activityLogToken;
            if (activityLogsToken is JArray)
            {
                var logsToken = activityLogsToken as JArray;
                activityLogToken = activityLogsToken[logsToken.Count - 1];
            }
            else
                activityLogToken = activityLogsToken;

            // give data priority over variables, duh
            JToken token = _variablesToken.Merge(activityLogToken["data"]);
            if (token.Type == JTokenType.Null)
                token = new JObject();

            using (var jsonReader = new JTokenReader(token))
            {
                return (T)SerializerCache.Deserializer.Deserialize(jsonReader, typeof(T));
            }
        }

        static IDictionary<string, object> GetEmptyObject()
        {
            return JsonConvert.DeserializeObject<IDictionary<string, object>>("{}");
        }


        class SanitizedActivity :
            Activity
        {
            public SanitizedActivity(Activity activity)
            {
                if (string.IsNullOrEmpty(activity.Name))
                    throw new SerializationException("An Activity Name is required");
                if (activity.Address == null)
                    throw new SerializationException("An Activity ExecuteAddress is required");

                Name = activity.Name;
                Address = activity.Address;
                Arguments = activity.Arguments ?? GetEmptyObject();
            }

            public string Name { get; private set; }
            public Uri Address { get; private set; }
            public IDictionary<string, object> Arguments { get; private set; }
        }


        class SanitizedActivityException :
            ActivityException
        {
            public SanitizedActivityException(ActivityException activityException)
            {
                if (string.IsNullOrEmpty(activityException.Name))
                    throw new SerializationException("An Activity Name is required");
                if (activityException.ExceptionInfo == null)
                    throw new SerializationException("An Activity ExceptionInfo is required");

                ExecutionId = activityException.ExecutionId;
                Timestamp = activityException.Timestamp;
                Duration = activityException.Duration;
                Name = activityException.Name;
                Host = activityException.Host;
                ExceptionInfo = activityException.ExceptionInfo;
            }

            public Guid ExecutionId { get; private set; }
            public DateTime Timestamp { get; private set; }
            public TimeSpan Duration { get; private set; }
            public string Name { get; private set; }
            public HostInfo Host { get; private set; }
            public ExceptionInfo ExceptionInfo { get; private set; }
        }


        class SanitizedActivityLog :
            ActivityLog
        {
            public SanitizedActivityLog(ActivityLog activityLog)
            {
                if (string.IsNullOrEmpty(activityLog.Name))
                    throw new SerializationException("An ActivityLog Name is required");

                ExecutionId = activityLog.ExecutionId;
                Name = activityLog.Name;
                Timestamp = activityLog.Timestamp;
                Duration = activityLog.Duration;
                Host = activityLog.Host;
            }

            public Guid ExecutionId { get; private set; }
            public string Name { get; private set; }
            public DateTime Timestamp { get; private set; }
            public TimeSpan Duration { get; private set; }
            public HostInfo Host { get; private set; }
        }


        class SanitizedCompensateLog :
            CompensateLog
        {
            public SanitizedCompensateLog(CompensateLog compensateLog)
            {
                if (compensateLog.Address == null)
                    throw new SerializationException("An CompensateLog CompensateAddress is required");

                ExecutionId = compensateLog.ExecutionId;
                Address = compensateLog.Address;
                Data = compensateLog.Data ?? GetEmptyObject();
            }

            public Guid ExecutionId { get; private set; }
            public Uri Address { get; private set; }
            public IDictionary<string, object> Data { get; private set; }
        }


        class SanitizedSubscription :
            Subscription
        {
            public SanitizedSubscription(Subscription subscription)
            {
                if (subscription.Address == null)
                    throw new SerializationException("A subscription address is required");

                Address = subscription.Address;
                Events = subscription.Events;
                Include = subscription.Include;
            }

            public Uri Address { get; private set; }
            public RoutingSlipEvents Events { get; private set; }
            public RoutingSlipEventContents Include { get; private set; }
        }
    }
}