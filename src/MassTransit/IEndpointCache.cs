// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Transports;


    /// <summary>
    /// The endpoint factory methods used to retrieve objects implementing IEndpoint from Uris.
    /// </summary>
    public interface IEndpointCache :
        IDisposable
    {
        /// <summary>
        /// Returns an IEndpoint for the Uri specified. If the endpoint has not yet been created,
        /// the factory will attempt to create an endpoint for the Uri.
        /// </summary>
        /// <param name="uri">The Uri to resolve to an endpoint</param>
        /// <returns>An IEndpoint instance - never null; if it cannot be gotten, an exception is thrown instead.</returns>
        /// <exception cref="EndpointException">Could not build up the endpoint with the 
        /// <see cref="IEndpointFactory"/> configured for the bus</exception>
        /// <exception cref="ConfigurationException">The scheme in the uri didn't have a corresponding
        /// <see cref="ITransportFactory"/> configured in the bus; the bus doesn't know how to
        /// send over such a scheme/transport protocol.</exception>
        IEndpoint GetEndpoint(Uri uri);
    }
}