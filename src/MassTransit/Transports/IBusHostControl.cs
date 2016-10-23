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
namespace MassTransit.Transports
{
    using System;


    public interface IBusHostControl :
        IBusHost
    {
        /// <summary>
        /// Starts the Host, which begins the connection asynchronously.
        /// </summary>
        /// <returns></returns>
        HostHandle Start();

        /// <summary>
        /// Returns true if the address matches the host
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        bool Matches(Uri address);
    }
}