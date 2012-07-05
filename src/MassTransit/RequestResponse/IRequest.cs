// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RequestResponse
{
    /// <summary>
    /// Handle to a request that was sent/published
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Identifies the request for matching up request/response messages
        /// </summary>
        string RequestId { get; }
    }

    /// <summary>
    /// Handle to a request that was sent/published
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRequest<out T> :
        IRequest
        where T : class
    {
        /// <summary>
        /// The request message that was sent/published
        /// </summary>
        T Message { get; }
    }
}