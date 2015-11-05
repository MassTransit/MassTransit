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
namespace MassTransit.Contracts
{
    using System;


    public interface ConcurrencyLimitUpdated
    {
        /// <summary>
        /// The actual time at which the adjustment was applied
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The identifier that was adjusted
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The current concurrency limit value
        /// </summary>
        int ConcurrencyLimit { get; }
    }
}