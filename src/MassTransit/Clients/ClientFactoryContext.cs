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
namespace MassTransit.Clients
{
    using System;
    using Pipeline;


    /// <summary>
    /// The client factory context, which contains multiple interfaces and properties used by clients
    /// </summary>
    public interface ClientFactoryContext :
        IConsumePipeConnector,
        IRequestPipeConnector,
        ISendEndpointProvider
    {
        /// <summary>
        /// Default timeout for requests
        /// </summary>
        RequestTimeout DefaultTimeout { get; }

        /// <summary>
        /// The address used for responses to messages sent by this client
        /// </summary>
        Uri ResponseAddress { get; }

        /// <summary>
        /// Return the publish endpoint for the client factory
        /// </summary>
        /// <value></value>
        IPublishEndpoint PublishEndpoint { get; }
    }
}
