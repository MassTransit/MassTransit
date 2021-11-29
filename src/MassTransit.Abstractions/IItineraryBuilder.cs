namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Courier.Contracts;


    public interface IItineraryBuilder
    {
        /// <summary>
        /// The tracking number of the routing slip
        /// </summary>
        Guid TrackingNumber { get; }

        /// <summary>
        /// Adds an activity to the routing slip without specifying any arguments
        /// </summary>
        /// <param name="name">The activity name</param>
        /// <param name="executeAddress">The execution address of the activity</param>
        void AddActivity(string name, Uri executeAddress);

        /// <summary>
        /// Adds an activity to the routing slip specifying activity arguments as an anonymous object
        /// </summary>
        /// <param name="name">The activity name</param>
        /// <param name="executeAddress">The execution address of the activity</param>
        /// <param name="arguments">An anonymous object of properties matching the argument names of the activity</param>
        void AddActivity(string name, Uri executeAddress, object arguments);

        /// <summary>
        /// Adds an activity to the routing slip specifying activity arguments a dictionary
        /// </summary>
        /// <param name="name">The activity name</param>
        /// <param name="executeAddress">The execution address of the activity</param>
        /// <param name="arguments">A dictionary of name/values matching the activity argument properties</param>
        void AddActivity(string name, Uri executeAddress, IDictionary<string, object> arguments);

        /// <summary>
        /// Add a variable to the routing slip
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void AddVariable(string key, string value);

        /// <summary>
        /// Add a variable to the routing slip
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void AddVariable(string key, object value);

        /// <summary>
        /// Sets the value of any existing variables to the value in the anonymous object,
        /// as well as adding any additional variables that did not exist previously.
        /// For example, SetVariables(new { IntValue = 27, StringValue = "Hello, World." });
        /// </summary>
        /// <param name="values"></param>
        void SetVariables(object values);

        /// <summary>
        /// Set multiple variables (from a dictionary, for example) on the routing slip
        /// </summary>
        /// <param name="values"></param>
        void SetVariables(IEnumerable<KeyValuePair<string, object>> values);

        /// <summary>
        /// Add the original itinerary to the routing slip (if present)
        /// </summary>
        /// <returns>The number of activities added to the itinerary</returns>
        int AddActivitiesFromSourceItinerary();

        /// <summary>
        /// Add an explicit subscription to the routing slip events
        /// </summary>
        /// <param name="address">The destination address where the events are sent</param>
        /// <param name="events">The events to include in the subscription</param>
        void AddSubscription(Uri address, RoutingSlipEvents events);

        /// <summary>
        /// Add an explicit subscription to the routing slip events
        /// </summary>
        /// <param name="address">The destination address where the events are sent</param>
        /// <param name="events">The events to include in the subscription</param>
        /// <param name="contents">The contents of the routing slip event</param>
        void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents);

        /// <summary>
        /// Adds a message subscription to the routing slip that will be sent at the specified event points
        /// </summary>
        /// <param name="address"></param>
        /// <param name="events"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        Task AddSubscription(Uri address, RoutingSlipEvents events, Func<ISendEndpoint, Task> callback);

        /// <summary>
        /// Adds a message subscription to the routing slip that will be sent at the specified event points
        /// </summary>
        /// <param name="address"></param>
        /// <param name="events"></param>
        /// <param name="contents"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        Task AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, Func<ISendEndpoint, Task> callback);

        /// <summary>
        /// Add an explicit subscription to the routing slip events
        /// </summary>
        /// <param name="address">The destination address where the events are sent</param>
        /// <param name="events">The events to include in the subscription</param>
        /// <param name="contents">The contents of the routing slip event</param>
        /// <param name="activityName">Only send events for the specified activity</param>
        void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, string activityName);

        /// <summary>
        /// Adds a message subscription to the routing slip that will be sent at the specified event points
        /// </summary>
        /// <param name="address"></param>
        /// <param name="events"></param>
        /// <param name="activityName">Only send events for the specified activity</param>
        /// <param name="contents"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        Task AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, string activityName, Func<ISendEndpoint, Task> callback);
    }
}
