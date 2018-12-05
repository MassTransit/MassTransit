// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Topology;
    using Transports;


    public interface IReadOnlyHostCollection<TConfiguration> :
        IReadOnlyHostCollection
        where TConfiguration : IHostConfiguration
    {
        /// <summary>
        /// Return an array of host configurations
        /// </summary>
        TConfiguration[] Hosts { get; }

        /// <summary>
        /// The number of hosts configured
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns the host at the specified index
        /// </summary>
        /// <param name="index"></param>
        TConfiguration this[int index] { get; }

        /// <summary>
        /// Try to get the host, by address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        bool TryGetHost(Uri address, out TConfiguration host);

        /// <summary>
        /// Try to get the host by reference, returning the host configuration
        /// </summary>
        /// <param name="host"></param>
        /// <param name="hostConfiguration"></param>
        /// <returns></returns>
        bool TryGetHost(IHost host, out TConfiguration hostConfiguration);
    }


    public interface IReadOnlyHostCollection :
        IEnumerable<IBusHostControl>
    {
        /// <summary>
        /// Return the bus topology, which is the first host's topology
        /// </summary>
        /// <returns></returns>
        IBusTopology GetBusTopology();

        /// <summary>
        /// Return the hosts so that they can be controlled.
        /// </summary>
        /// <returns></returns>
        IBusHostControl[] GetHosts();

        /// <summary>
        /// Try to get the host, by address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        bool TryGetHost(Uri address, out IHostConfiguration host);
    }
}