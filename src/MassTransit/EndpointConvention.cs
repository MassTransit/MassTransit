// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;


    public static class EndpointConvention
    {
        /// <summary>
        /// Map the message type to the specified address
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destinationAddress"></param>
        public static void Map<T>(Uri destinationAddress)
            where T : class
        {
            EndpointConventionCache<T>.Map(destinationAddress);
        }

        /// <summary>
        /// Map the message type to the endpoint returned by the specified method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpointAddressProvider"></param>
        public static void Map<T>(EndpointAddressProvider<T> endpointAddressProvider)
            where T : class
        {
            EndpointConventionCache<T>.Map(endpointAddressProvider);
        }

        public static bool TryGetDestinationAddress<T>(out Uri destinationAddress)
            where T : class
        {
            return EndpointConventionCache<T>.TryGetEndpointAddress(out destinationAddress);
        }

        public static bool TryGetDestinationAddress(Type messageType, out Uri destinationAddress)
        {
            return EndpointConventionCache.TryGetEndpointAddress(messageType, out destinationAddress);
        }
    }
}
