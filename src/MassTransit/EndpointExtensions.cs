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
namespace MassTransit
{
    public static class EndpointExtensions
    {
        /// <summary>
        /// Forwards the message to the endpoint
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint">The destination endpoint to which the message should be sent</param>
        /// <param name="context">The message context</param>
        /// <param name="message">The message to forward to the endpoint</param>
        public static void Forward<T>(this IEndpoint endpoint, IConsumeContext context, T message)
            where T : class
        {
            endpoint.Send(message, x => x.ForwardUsingOriginalContext(context));
        }

        /// <summary>
        /// Forwards the message to the endpoint
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint">The destination endpoint to which the message should be sent</param>
        /// <param name="context">The message context</param>
        public static void Forward<T>(this IEndpoint endpoint, IConsumeContext<T> context)
            where T : class
        {
            endpoint.Send(context.Message, x => x.ForwardUsingOriginalContext(context));
        }
    }
}