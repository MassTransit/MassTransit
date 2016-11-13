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
namespace MassTransit.Turnout.Configuration
{
    using System;


    public interface ITurnoutServiceConfigurator<T> :
        IReceiveEndpointConfigurator
        where T : class
    {
        IJobFactory<T> JobFactory { set; }

        TimeSpan SuperviseInterval { set; }

        /// <summary>
        /// Sets the number of partitions which can be used to process job commands
        /// concurrently. Note that this does not set the maximum number of jobs 
        /// executing, but just defines the concurrency for the consumers.
        /// </summary>
        int PartitionCount { set; }
    }
}