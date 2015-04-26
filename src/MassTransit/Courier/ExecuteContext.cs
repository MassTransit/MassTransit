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


    public interface ExecuteContext :
        PipeContext,
        IPublishEndpoint,
        ISendEndpointProvider
    {
        /// <summary>
        /// The tracking number for this routing slip
        /// </summary>
        Guid TrackingNumber { get; }

        /// <summary>
        /// The tracking number for this activity within the routing slip
        /// </summary>
        Guid ExecutionId { get; }

        HostInfo Host { get; }

        DateTime Timestamp { get; }

        TimeSpan Elapsed { get; }

        ConsumeContext ConsumeContext { get; }

        string ActivityName { get; }

        /// <summary>
        /// Completes the execution, without passing a compensating log entry
        /// </summary>
        /// <returns></returns>
        ExecutionResult Completed();

        /// <summary>
        /// Completes the execution, passing updated variables to the routing slip
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        ExecutionResult CompletedWithVariables(IEnumerable<KeyValuePair<string, object>> variables);

        /// <summary>
        /// Completes the execution, passing updated variables to the routing slip
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        ExecutionResult CompletedWithVariables(object variables);

        /// <summary>
        /// Completes the activity, passing a compensation log entry
        /// </summary>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="log"></param>
        /// <returns></returns>
        ExecutionResult Completed<TLog>(TLog log)
            where TLog : class;

        /// <summary>
        /// Completes the activity, passing a compensation log entry and additional variables to set on 
        /// the routing slip
        /// </summary>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="log"></param>
        /// <param name="variables">An anonymous object of values to add/set as variables on the routing slip</param>
        /// <returns></returns>
        ExecutionResult CompletedWithVariables<TLog>(TLog log, object variables)
            where TLog : class;

        /// <summary>
        /// Completes the activity, passing a compensation log entry and additional variables to set on 
        /// the routing slip
        /// </summary>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="log"></param>
        /// <param name="variables">An dictionary of values to add/set as variables on the routing slip</param>
        /// <returns></returns>
        ExecutionResult CompletedWithVariables<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables)
            where TLog : class;

        ExecutionResult ReviseItinerary(Action<ItineraryBuilder> buildItinerary);

        ExecutionResult ReviseItinerary<TLog>(TLog log, Action<ItineraryBuilder> buildItinerary)
            where TLog : class;

        ExecutionResult ReviseItinerary<TLog>(TLog log, object variables, Action<ItineraryBuilder> buildItinerary)
            where TLog : class;

        ExecutionResult ReviseItinerary<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables,
            Action<ItineraryBuilder> buildItinerary)
            where TLog : class;

        /// <summary>
        /// The activity Faulted for an unknown reason, but compensation should be triggered
        /// </summary>
        /// <returns></returns>
        ExecutionResult Faulted();

        /// <summary>
        /// The activity Faulted, and compensation should be triggered
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        ExecutionResult Faulted(Exception exception);
    }


    public interface ExecuteContext<out TArguments> :
        ExecuteContext
        where TArguments : class
    {
        /// <summary>
        /// The arguments from the routing slip for this activity
        /// </summary>
        TArguments Arguments { get; }
    }
}