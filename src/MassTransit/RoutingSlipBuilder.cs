namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using Courier.Messages;
    using Events;
    using Serialization;
    using Util;


    /// <summary>
    /// A RoutingSlipBuilder is used to create a routing slip with proper validation that the resulting RoutingSlip
    /// is valid.
    /// </summary>
    public class RoutingSlipBuilder :
        IRoutingSlipBuilder,
        IRoutingSlipSendEndpointTarget
    {
        public static readonly IDictionary<string, object> NoArguments = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        readonly IList<ActivityException> _activityExceptions;
        readonly IList<ActivityLog> _activityLogs;
        readonly IList<CompensateLog> _compensateLogs;
        readonly DateTime _createTimestamp;
        readonly IList<Activity> _itinerary;
        readonly List<Activity> _sourceItinerary;
        readonly IList<Subscription> _subscriptions;
        readonly IDictionary<string, object> _variables;

        public RoutingSlipBuilder(Guid trackingNumber)
        {
            TrackingNumber = trackingNumber;
            _createTimestamp = DateTime.UtcNow;

            _itinerary = new List<Activity>();
            _sourceItinerary = new List<Activity>();
            _activityLogs = new List<ActivityLog>();
            _activityExceptions = new List<ActivityException>();
            _compensateLogs = new List<CompensateLog>();
            _variables = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            _subscriptions = new List<Subscription>();
        }

        public RoutingSlipBuilder(RoutingSlip routingSlip, Func<IEnumerable<Activity>, IEnumerable<Activity>> activitySelector)
        {
            TrackingNumber = routingSlip.TrackingNumber;
            _createTimestamp = routingSlip.CreateTimestamp;
            _itinerary = new List<Activity>(activitySelector(routingSlip.Itinerary));
            _activityLogs = new List<ActivityLog>(routingSlip.ActivityLogs);
            _compensateLogs = new List<CompensateLog>(routingSlip.CompensateLogs);
            _activityExceptions = new List<ActivityException>(routingSlip.ActivityExceptions);
            _variables = new Dictionary<string, object>(routingSlip.Variables, StringComparer.OrdinalIgnoreCase);
            _subscriptions = new List<Subscription>(routingSlip.Subscriptions);

            _sourceItinerary = new List<Activity>();
        }

        public RoutingSlipBuilder(RoutingSlip routingSlip, IEnumerable<Activity> itinerary, IEnumerable<Activity> sourceItinerary)
        {
            TrackingNumber = routingSlip.TrackingNumber;
            _createTimestamp = routingSlip.CreateTimestamp;
            _itinerary = new List<Activity>(itinerary);
            _activityLogs = new List<ActivityLog>(routingSlip.ActivityLogs);
            _compensateLogs = new List<CompensateLog>(routingSlip.CompensateLogs);
            _activityExceptions = new List<ActivityException>(routingSlip.ActivityExceptions);
            _variables = new Dictionary<string, object>(routingSlip.Variables, StringComparer.OrdinalIgnoreCase);
            _subscriptions = new List<Subscription>(routingSlip.Subscriptions);

            _sourceItinerary = new List<Activity>(sourceItinerary);
        }

        public RoutingSlipBuilder(RoutingSlip routingSlip, IEnumerable<CompensateLog> compensateLogs)
        {
            TrackingNumber = routingSlip.TrackingNumber;
            _createTimestamp = routingSlip.CreateTimestamp;
            _itinerary = new List<Activity>(routingSlip.Itinerary);
            _activityLogs = new List<ActivityLog>(routingSlip.ActivityLogs);
            _compensateLogs = new List<CompensateLog>(compensateLogs);
            _activityExceptions = new List<ActivityException>(routingSlip.ActivityExceptions);
            _variables = new Dictionary<string, object>(routingSlip.Variables, StringComparer.OrdinalIgnoreCase);
            _subscriptions = new List<Subscription>(routingSlip.Subscriptions);

            _sourceItinerary = new List<Activity>();
        }

        public IList<Activity> SourceItinerary => _sourceItinerary;

        /// <summary>
        /// The tracking number of the routing slip
        /// </summary>
        public Guid TrackingNumber { get; }

        /// <summary>
        /// Adds an activity to the routing slip without specifying any arguments
        /// </summary>
        /// <param name="name">The activity name</param>
        /// <param name="executeAddress">The execution address of the activity</param>
        public void AddActivity(string name, Uri executeAddress)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (executeAddress == null)
                throw new ArgumentNullException(nameof(executeAddress));

            Activity activity = new RoutingSlipActivity(name, executeAddress, NoArguments);
            _itinerary.Add(activity);
        }

        /// <summary>
        /// Adds an activity to the routing slip specifying activity arguments as an anonymous object
        /// </summary>
        /// <param name="name">The activity name</param>
        /// <param name="executeAddress">The execution address of the activity</param>
        /// <param name="arguments">An anonymous object of properties matching the argument names of the activity</param>
        public void AddActivity(string name, Uri executeAddress, object arguments)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (executeAddress == null)
                throw new ArgumentNullException(nameof(executeAddress));
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            IDictionary<string, object> argumentsDictionary = GetObjectAsDictionary(arguments);

            Activity activity = new RoutingSlipActivity(name, executeAddress, argumentsDictionary);
            _itinerary.Add(activity);
        }

        /// <summary>
        /// Adds an activity to the routing slip specifying activity arguments a dictionary
        /// </summary>
        /// <param name="name">The activity name</param>
        /// <param name="executeAddress">The execution address of the activity</param>
        /// <param name="arguments">A dictionary of name/values matching the activity argument properties</param>
        public void AddActivity(string name, Uri executeAddress, IDictionary<string, object> arguments)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (executeAddress == null)
                throw new ArgumentNullException(nameof(executeAddress));
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            Activity activity = new RoutingSlipActivity(name, executeAddress, arguments);
            _itinerary.Add(activity);
        }

        /// <summary>
        /// Add a string value to the routing slip
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddVariable(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrEmpty(value))
                _variables.Remove(key);
            else
                _variables[key] = value;
        }

        /// <summary>
        /// Add an object variable to the routing slip
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddVariable(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null || value is string s && string.IsNullOrEmpty(s))
                _variables.Remove(key);
            else
                _variables[key] = value;
        }

        /// <summary>
        /// Sets the value of any existing variables to the value in the anonymous object,
        /// as well as adding any additional variables that did not exist previously.
        /// For example, SetVariables(new { IntValue = 27, StringValue = "Hello, World." });
        /// </summary>
        /// <param name="values"></param>
        public void SetVariables(object values)
        {
            IDictionary<string, object> dictionary = GetObjectAsDictionary(values);

            SetVariablesFromDictionary(dictionary);
        }

        public void SetVariables(IEnumerable<KeyValuePair<string, object>> values)
        {
            SetVariablesFromDictionary(values);
        }

        /// <summary>
        /// Adds the activities from the source itinerary to the new routing slip and removes them from the
        /// source itinerary.
        /// </summary>
        /// <returns></returns>
        public int AddActivitiesFromSourceItinerary()
        {
            var count = _sourceItinerary.Count;

            foreach (var activity in _sourceItinerary)
                _itinerary.Add(activity);

            _sourceItinerary.Clear();

            return count;
        }

        /// <summary>
        /// Add an explicit subscription to the routing slip events
        /// </summary>
        /// <param name="address">The destination address where the events are sent</param>
        /// <param name="events">The events to include in the subscription</param>
        public void AddSubscription(Uri address, RoutingSlipEvents events)
        {
            _subscriptions.Add(new RoutingSlipSubscription(address, events, RoutingSlipEventContents.All));
        }

        /// <summary>
        /// Add an explicit subscription to the routing slip events
        /// </summary>
        /// <param name="address">The destination address where the events are sent</param>
        /// <param name="events">The events to include in the subscription</param>
        /// <param name="contents">The contents of the routing slip event</param>
        public void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents)
        {
            _subscriptions.Add(new RoutingSlipSubscription(address, events, contents));
        }

        /// <summary>
        /// Add an explicit subscription to the routing slip events
        /// </summary>
        /// <param name="address">The destination address where the events are sent</param>
        /// <param name="events">The events to include in the subscription</param>
        /// <param name="contents">The contents of the routing slip event</param>
        /// <param name="activityName">Only send events for the specified activity</param>
        public void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, string activityName)
        {
            _subscriptions.Add(new RoutingSlipSubscription(address, events, contents, activityName));
        }

        /// <summary>
        /// Adds a message subscription to the routing slip that will be sent at the specified event points
        /// </summary>
        /// <param name="address"></param>
        /// <param name="events"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task AddSubscription(Uri address, RoutingSlipEvents events, Func<ISendEndpoint, Task> callback)
        {
            return callback(new RoutingSlipBuilderSendEndpoint(this, address, events, null));
        }

        /// <summary>
        /// Adds a message subscription to the routing slip that will be sent at the specified event points
        /// </summary>
        /// <param name="address"></param>
        /// <param name="events"></param>
        /// <param name="contents"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, Func<ISendEndpoint, Task> callback)
        {
            return callback(new RoutingSlipBuilderSendEndpoint(this, address, events, null, contents));
        }

        /// <summary>
        /// Adds a message subscription to the routing slip that will be sent at the specified event points
        /// </summary>
        /// <param name="address"></param>
        /// <param name="events"></param>
        /// <param name="activityName">Only send events for the specified activity</param>
        /// <param name="contents"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, string activityName,
            Func<ISendEndpoint, Task> callback)
        {
            return callback(new RoutingSlipBuilderSendEndpoint(this, address, events, activityName, contents));
        }

        /// <summary>
        /// Adds a custom subscription message to the routing slip which is sent at the specified events
        /// </summary>
        /// <param name="address">The destination address where the events are sent</param>
        /// <param name="events">The events to include in the subscription</param>
        /// <param name="contents">The contents of the routing slip event</param>
        /// <param name="activityName"></param>
        /// <param name="message">The custom message to be sent</param>
        void IRoutingSlipSendEndpointTarget.AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, string activityName,
            MessageEnvelope message)
        {
            _subscriptions.Add(new RoutingSlipSubscription(address, events, contents, activityName, message));
        }

        /// <summary>
        /// Builds the routing slip using the current state of the builder
        /// </summary>
        /// <returns>The RoutingSlip</returns>
        public RoutingSlip Build()
        {
            return new RoutingSlipRoutingSlip(TrackingNumber, _createTimestamp, _itinerary, _activityLogs, _compensateLogs, _activityExceptions,
                _variables, _subscriptions);
        }

        public void AddActivityLog(HostInfo host, string name, Guid activityTrackingNumber, DateTime timestamp, TimeSpan duration)
        {
            _activityLogs.Add(new RoutingSlipActivityLog(host, activityTrackingNumber, name, timestamp, duration));
        }

        public void AddCompensateLog(Guid activityTrackingNumber, Uri compensateAddress, object logObject)
        {
            IDictionary<string, object> data = GetObjectAsDictionary(logObject);

            _compensateLogs.Add(new RoutingSlipCompensateLog(activityTrackingNumber, compensateAddress, data));
        }

        public void AddCompensateLog(Guid activityTrackingNumber, Uri compensateAddress, IDictionary<string, object> data)
        {
            _compensateLogs.Add(new RoutingSlipCompensateLog(activityTrackingNumber, compensateAddress, data));
        }

        /// <summary>
        /// Adds an activity exception to the routing slip
        /// </summary>
        /// <param name="host"></param>
        /// <param name="name">The name of the faulted activity</param>
        /// <param name="activityTrackingNumber">The activity tracking number</param>
        /// <param name="timestamp">The timestamp of the exception</param>
        /// <param name="elapsed">The time elapsed from the start of the activity to the exception</param>
        /// <param name="exception">The exception thrown by the activity</param>
        public void AddActivityException(HostInfo host, string name, Guid activityTrackingNumber, DateTime timestamp, TimeSpan elapsed,
            Exception exception)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            var exceptionInfo = new FaultExceptionInfo(exception);

            ActivityException activityException = new RoutingSlipActivityException(name, host, activityTrackingNumber, timestamp, elapsed,
                exceptionInfo);
            _activityExceptions.Add(activityException);
        }

        /// <summary>
        /// Adds an activity exception to the routing slip
        /// </summary>
        /// <param name="host"></param>
        /// <param name="name">The name of the faulted activity</param>
        /// <param name="activityTrackingNumber">The activity tracking number</param>
        /// <param name="timestamp">The timestamp of the exception</param>
        /// <param name="elapsed">The time elapsed from the start of the activity to the exception</param>
        /// <param name="exceptionInfo"></param>
        public void AddActivityException(HostInfo host, string name, Guid activityTrackingNumber, DateTime timestamp, TimeSpan elapsed,
            ExceptionInfo exceptionInfo)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (exceptionInfo == null)
                throw new ArgumentNullException(nameof(exceptionInfo));

            ActivityException activityException = new RoutingSlipActivityException(name, host, activityTrackingNumber, timestamp, elapsed, exceptionInfo);
            _activityExceptions.Add(activityException);
        }

        public void AddActivityException(ActivityException activityException)
        {
            if (activityException == null)
                throw new ArgumentNullException(nameof(activityException));

            _activityExceptions.Add(activityException);
        }

        void SetVariablesFromDictionary(IEnumerable<KeyValuePair<string, object>> values)
        {
            foreach (KeyValuePair<string, object> value in values)
            {
                if (value.Value == null || value.Value is string s && string.IsNullOrEmpty(s))
                    _variables.Remove(value.Key);
                else
                    _variables[value.Key] = value.Value;
            }
        }

        public static IDictionary<string, object> GetObjectAsDictionary(object values)
        {
            return ConvertObject.ToDictionary(values);
        }
    }
}
