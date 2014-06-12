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
    using System.Threading;
    using System.Threading.Tasks;


    public interface ConsumeContext :
        MessageContext
    {
        /// <summary>
        /// The cancellation token that is cancelled when the receive context is cancelled
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// The original receive context
        /// </summary>
        ReceiveContext ReceiveContext { get; }

        /// <summary>
        ///     Returns true if the specified message type is contained in the serialized message
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        bool HasMessageType(Type messageType);

        /// <summary>
        ///     Returns the specified message type if available, otherwise returns false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="consumeContext"></param>
        /// <returns></returns>
        bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class;

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or 
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="message">The message to send in response</param>
        Task<SentContext> RespondAsync<T>(T message)
            where T : class;

        /// <summary>
        /// Adds a response to the message being consumed, which will be sent once the consumer
        /// has completed. The message is not acknowledged until the response is acknowledged.
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="message">The message to send in response</param>
        void Respond<T>(T message)
            where T : class;

        /// <summary>
        /// Resend the message to the end of the queue with the RetryCount incremented
        /// </summary>
        void RetryLater();

        /// <summary>
        /// Return an endpoint using the specified address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        IEndpoint GetEndpoint(Uri address);

        /// <summary>
        /// Notify that the message has been consumed
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="messageType">The message type</param>
        /// <param name="consumerType">The consumer type</param>
        void NotifyConsumed(TimeSpan elapsed, string messageType, string consumerType);

        /// <summary>
        /// Notify that a message consumer has faulted
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="consumerType"></param>
        /// <param name="exception"></param>
        void NotifyFaulted(string messageType, string consumerType, Exception exception);
    }


    public interface ConsumeContext<out T> :
        ConsumeContext
        where T : class
    {
        T Message { get; }

        /// <summary>
        /// Notify that the message has been consumed
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="consumerType">The consumer type</param>
        void NotifyConsumed(TimeSpan elapsed, string consumerType);

        /// <summary>
        /// Notify that a fault occurred during message consumption
        /// </summary>
        /// <param name="consumerType"></param>
        /// <param name="exception"></param>
        void NotifyFaulted(string consumerType, Exception exception);

    }
}