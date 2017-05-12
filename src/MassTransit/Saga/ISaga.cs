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
namespace MassTransit.Saga
{
    using Newtonsoft.Json;
    using System;


    /// <summary>
    /// Interface that specifies a class is usable as a saga instance, including
    /// the ability to get and set the CorrelationId on the saga instance.
    /// </summary>
    public interface ISaga
    {
        /// <summary>
        /// Identifies the saga instance uniquely, and is the primary correlation
        /// for the instance. While the setter is not typically called, it is there
        /// to support persistence consistently across implementations.
        /// </summary>
        [JsonProperty("id")]
        Guid CorrelationId { get; set; }
    }
}