// Copyright 2007-2013 Chris Patterson
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
namespace MassTransit.Steward
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Metadata;
    using Util;


    public static class DispatchCommandExtensions
    {
        /// <summary>
        /// Dispatches a command to a service
        /// </summary>
        /// <typeparam name="T">The command type</typeparam>
        /// <param name="endpoint">The dispatcher endpoint</param>
        /// <param name="payload">The command</param>
        /// <param name="destination">The destination address of the service endpoint</param>
        /// <param name="resources">The resources required by the command</param>
        /// <returns>A handle to the dispatch</returns>
        public static async Task<DispatchMessageHandle<T>> DispatchMessage<T>(this ISendEndpoint endpoint, T payload, Uri destination,
            params Uri[] resources)
            where T : class
        {
            var message = new DispatchCommandMessage<T>(destination, payload, resources);

            await endpoint.Send<DispatchMessage<T>>(message);

            return new DispatchedMessageHandle<T>(message.DispatchId, message.CreateTime, message.Destination, payload);
        }


        class DispatchCommandMessage<T> :
            DispatchMessage<T>
            where T : class
        {
            public DispatchCommandMessage(Uri destination, T payload, IEnumerable<Uri> resources)
            {
                if (destination == null)
                    throw new ArgumentNullException("destination");
                if (payload == null)
                    throw new ArgumentNullException("payload");
                if (resources == null)
                    throw new ArgumentNullException("resources");

                Destination = destination;
                Payload = payload;

                DispatchId = NewId.NextGuid();
                CreateTime = DateTime.UtcNow;

                PayloadType = TypeMetadataCache<T>.MessageTypeNames;

                Resources = resources.ToArray();
            }

            public Guid DispatchId { get; private set; }
            public DateTime CreateTime { get; private set; }
            public Uri[] Resources { get; private set; }
            public string[] PayloadType { get; private set; }
            public Uri Destination { get; private set; }
            public T Payload { get; private set; }
        }


        class DispatchedMessageHandle<T> :
            DispatchMessageHandle<T>
            where T : class
        {
            public DispatchedMessageHandle(Guid dispatchId, DateTime createTime, Uri destination, T payload)
            {
                DispatchId = dispatchId;
                CreateTime = createTime;
                Destination = destination;
                Payload = payload;
            }

            public Guid DispatchId { get; private set; }
            public DateTime CreateTime { get; private set; }
            public Uri Destination { get; private set; }
            public T Payload { get; private set; }
        }
    }
}