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
    using System;
    using Util;

    /// <summary>
    /// Extensions methods for parameterizing message contexts
    /// </summary>
    public static class MessageContextExtensions
    {
        /// <summary>
        /// Sets the source address of the message to be send.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="uriString">The URI string.</param>
        public static void SetSourceAddress<T>(this ISendContext<T> context, [NotNull] string uriString)
            where T : class
        {
            if (uriString == null)
                throw new ArgumentNullException("uriString");
            context.SetSourceAddress(uriString.ToUri());
        }

        /// <summary>
        /// Sets the destination address of the message to be send.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="uriString">The URI string.</param>
        public static void SetDestinationAddress<T>(this ISendContext<T> context, [NotNull] string uriString)
            where T : class
        {
            if (uriString == null)
                throw new ArgumentNullException("uriString");
            context.SetDestinationAddress(uriString.ToUri());
        }

        /// <summary>
        /// Sets the response address of the message to be send.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="uriString">The URI string.</param>
        public static void SetResponseAddress<T>(this ISendContext<T> context, [NotNull] string uriString)
            where T : class
        {
            if (uriString == null)
                throw new ArgumentNullException("uriString");

            context.SetResponseAddress(uriString.ToUri());
        }

        /// <summary>
        /// Sets the response address of the message to be send to the <see cref="IEndpoint"/> of the given <see cref="IServiceBus"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="bus">The bus.</param>
        public static void SendResponseTo<T>(this ISendContext<T> context, [NotNull] IServiceBus bus)
            where T : class
        {
            if (bus == null)
                throw new ArgumentNullException("bus");

            context.SetResponseAddress(bus.Endpoint.Address.Uri);
        }

        /// <summary>
        /// Sets the response address of the message to be send to the given <see cref="IEndpoint"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="endpoint">The endpoint.</param>
        public static void SendResponseTo<T>(this ISendContext<T> context, [NotNull] IEndpoint endpoint)
            where T : class
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            context.SetResponseAddress(endpoint.Address.Uri);
        }

        /// <summary>
        /// Sets the response address of the message to be send to the given <see cref="Uri"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="uri">The URI.</param>
        public static void SendResponseTo<T>(this ISendContext<T> context, Uri uri)
            where T : class
        {
            context.SetResponseAddress(uri);
        }

        /// <summary>
        /// Sets the fault address of the message to be send to the given uri.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="uriString">The URI string.</param>
        public static void SetFaultAddress<T>(this ISendContext<T> context, [NotNull] string uriString)
            where T : class
        {
            if (uriString == null)
                throw new ArgumentNullException("uriString");
            context.SetFaultAddress(uriString.ToUri());
        }

        /// <summary>
        /// Sets the fault address of the message to be send to the given <see cref="IServiceBus"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="bus">The bus.</param>
        public static void SendFaultTo<T>(this ISendContext<T> context, [NotNull] IServiceBus bus)
            where T : class
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            context.SetFaultAddress(bus.Endpoint.Address.Uri);
        }

        /// <summary>
        /// Sets the fault address of the message to be send to the given <see cref="IEndpoint"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="endpoint">The endpoint.</param>
        public static void SendFaultTo<T>(this ISendContext<T> context, [NotNull] IEndpoint endpoint)
            where T : class
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");
            context.SetFaultAddress(endpoint.Address.Uri);
        }

        /// <summary>
        /// Sets the fault address of the message to be send to the given <see cref="Uri"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="uri">The URI.</param>
        public static void SendFaultTo<T>(this ISendContext<T> context, [NotNull] Uri uri)
            where T : class
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            context.SetFaultAddress(uri);
        }

        /// <summary>
        /// Indicates that the messag to be send expireses at the given <see cref="DateTime"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="value">The value.</param>
        public static void ExpiresAt<T>(this ISendContext<T> context, DateTime value)
            where T : class
        {
            context.SetExpirationTime(value);
        }

        /// <summary>
        /// Sets the message to expire after the specified TimeSpan.
        /// </summary>
        /// <param name="context">The send context of the message</param>
        /// <param name="value">The span of time until the message expires</param>
        public static void ExpiresIn<T>(this ISendContext<T> context, TimeSpan value) 
            where T : class
        {
            context.SetExpirationTime(DateTime.UtcNow + value);
        }

        /// <summary>
        /// Sets the type of the message to be send.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="messageType">Type of the message.</param>
        public static void SetMessageType<T>(this ISendContext<T> context, [NotNull] Type messageType)
            where T : class
        {
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            context.SetMessageType(messageType.ToMessageName());
        }

        /// <summary>
        /// Sets the input address of the message to be send to the given <see cref="IEndpointAddress"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="address">The address.</param>
        public static void SetInputAddress(this IReceiveContext context, [NotNull] IEndpointAddress address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            context.SetInputAddress(address.Uri);
        }

        /// <summary>
        /// Sends the given message to the specified context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="message">The message.</param>
        public static void Respond<T>(this IConsumeContext context, T message)
            where T : class
        {
            context.Respond(message, x => { });
        }
    }
}