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
namespace Automatonymous
{
    using System;


    public interface IRequestConfigurator<T, TRequest, TResponse>
        where T : class
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// Sets the service address of the request handler
        /// </summary>
        Uri ServiceAddress { set; }

        /// <summary>
        /// Sets the address of the message scheduling service
        /// </summary>
        Uri SchedulingServiceAddress { set; }

        /// <summary>
        /// Sets the request timeout
        /// </summary>
        TimeSpan Timeout { set; }
    }
}