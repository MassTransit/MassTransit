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
namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Pipeline;


    public static class SendEndpointExtensions
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, T message)
            where T : class
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, T message, IPipe<SendContext<T>> pipe)
            where T : class
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, pipe, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, T message, IPipe<SendContext> pipe)
            where T : class
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, pipe, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ConsumeContext context, Uri destinationAddress, object message)
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="messageType"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ConsumeContext context, Uri destinationAddress, object message, Type messageType)
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, messageType, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="messageType"></param>
        /// <param name="pipe"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ConsumeContext context, Uri destinationAddress, object message, Type messageType, IPipe<SendContext> pipe)
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, messageType, pipe, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ConsumeContext context, Uri destinationAddress, object message, IPipe<SendContext> pipe)
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, pipe, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="values"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, object values)
            where T : class
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send<T>(values, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="values"></param>
        /// <param name="pipe"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, object values, IPipe<SendContext<T>> pipe)
            where T : class
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(values, pipe, context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="values"></param>
        /// <param name="pipe"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ConsumeContext context, Uri destinationAddress, object values, IPipe<SendContext> pipe)
            where T : class
        {
            ISendEndpoint endpoint = await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send<T>(values, pipe, context.CancellationToken).ConfigureAwait(false);
        }
    }
}