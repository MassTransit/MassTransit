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
namespace Automatonymous.Events
{
    using System;


    public interface RequestTimeoutExpired<TRequest> where TRequest : class
    {
        /// <summary>
        /// The correlationId of the state machine
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// When the request expired
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The expiration time that was scheduled for the request
        /// </summary>
        DateTime ExpirationTime { get; }

        /// <summary>
        /// The requestId of the request
        /// </summary>
        Guid RequestId { get; }
    }
}