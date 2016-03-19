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
namespace MassTransit.Context
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Pipeline;
    using Util;


    /// <summary>
    /// Converts the object message type to the generic type T and publishes it on the endpoint specified.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PublishEndpointConverter<T> :
        IPublishEndpointConverter
        where T : class
    {
        Task IPublishEndpointConverter.Publish(IPublishEndpoint endpoint, object message, CancellationToken cancellationToken)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var msg = message as T;
            if (msg == null)
                throw new ArgumentException("Unexpected message type: " + TypeMetadataCache.GetShortName(message.GetType()));

            return endpoint.Publish(msg, cancellationToken);
        }

        Task IPublishEndpointConverter.Publish(IPublishEndpoint endpoint, object message, IPipe<PublishContext> pipe,
            CancellationToken cancellationToken)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var msg = message as T;
            if (msg == null)
                throw new ArgumentException("Unexpected message type: " + TypeMetadataCache.GetShortName(message.GetType()));

            return endpoint.Publish(msg, pipe, cancellationToken);
        }
    }
}