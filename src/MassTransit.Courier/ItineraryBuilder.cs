// Copyright 2007-2013 Chris Patterson
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


    public interface ItineraryBuilder
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
        /// 
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
        int AddSourceItinerary();
    }
}