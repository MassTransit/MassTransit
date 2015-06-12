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
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A Bus Host is a transport-neutral reference to a host
    /// </summary>
    public interface IBusHost
    {
    }


    public interface IBusHostControl :
        IBusHost
    {
        /// <summary>
        /// Starts the Host, initiating the connection.
        /// TODO maybe this should be Task&lt;HostHandle&gt; after all 
        /// </summary>
        /// <returns></returns>
        HostHandle Start();
    }


    public interface HostHandle :
        IDisposable
    {
        /// <summary>
        /// Close the Host, shutting it down for good.
        /// </summary>
        /// <returns></returns>
        Task Stop(CancellationToken cancellationToken = default(CancellationToken));
    }
}