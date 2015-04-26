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


    public interface CompensateContext :
        PipeContext,
        IPublishEndpoint,
        ISendEndpointProvider
    {
        /// <summary>
        /// The tracking number for this routing slip
        /// </summary>
        Guid TrackingNumber { get; }

        /// <summary>
        /// The host performing the compensation
        /// </summary>
        HostInfo Host { get; }

        /// <summary>
        /// The start time for the activity compensation
        /// </summary>
        DateTime StartTimestamp { get; }

        /// <summary>
        /// The time elapsed for the compensation operation
        /// </summary>
        TimeSpan ElapsedTime { get; }

        /// <summary>
        /// The consume context of the compensation routing slip
        /// </summary>
        ConsumeContext ConsumeContext { get; }

        /// <summary>
        /// The name of the activity being compensated
        /// </summary>
        string ActivityName { get; }

        /// <summary>
        /// The tracking number for this activity within the routing slip
        /// </summary>
        Guid ExecutionId { get; }

        /// <summary>
        /// The compensation was successful
        /// </summary>
        /// <returns></returns>
        CompensationResult Compensated();

        /// <summary>
        /// The compenstation was successful
        /// </summary>
        /// <param name="values">The variables to be updated on the routing slip</param>
        /// <returns></returns>
        CompensationResult Compensated(object values);

        /// <summary>
        /// The compensation was successful
        /// </summary>
        /// <param name="variables">The variables to be updated on the routing slip</param>
        /// <returns></returns>
        CompensationResult Compensated(IDictionary<string, object> variables);

        /// <summary>
        /// The compensation failed
        /// </summary>
        /// <returns></returns>
        CompensationResult Failed();

        /// <summary>
        /// The compensation failed with the specified exception
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        CompensationResult Failed(Exception exception);
    }


    public interface CompensateContext<out TLog> :
        CompensateContext
        where TLog : class
    {
        TLog Log { get; }
    }
}