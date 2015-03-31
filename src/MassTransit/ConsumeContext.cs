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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Pipeline;


    public interface ConsumeContext :
        MessageContext,
        IPublishEndpoint
    {
        /// <summary>
        /// The original receive context
        /// </summary>
        ReceiveContext ReceiveContext { get; }

        /// <summary>
        /// An awaitable task that is completed once the consume context is completed
        /// </summary>
        Task CompleteTask { get; }

        /// <summary>
        /// Returns the supported message types from the message
        /// </summary>
        IEnumerable<string> SupportedMessageTypes { get; }

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
        Task RespondAsync<T>(T message)
            where T : class;

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or 
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="message">The message to send in response</param>
        /// <param name="sendPipe">The pipe used to customize the response send context</param>
        Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
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
        /// Return an endpoint to which messages can be sent
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<ISendEndpoint> GetSendEndpoint(Uri address);

        /// <summary>
        /// Notify that the message has been consumed
        /// </summary>
        /// <param name="context"></param>
        /// <param name="duration"></param>
        /// <param name="consumerType">The consumer type</param>
        void NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class;

        /// <summary>
        /// Notify that a message consumer has faulted
        /// </summary>
        /// <param name="context"></param>
        /// <param name="duration"></param>
        /// <param name="consumerType">The message consumer type</param>
        /// <param name="exception">The exception that occurred</param>
        void NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class;
    }


    public interface ConsumeContext<out T> :
        ConsumeContext
        where T : class
    {
        T Message { get; }

        /// <summary>
        /// Notify that the message has been consumed
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="consumerType">The consumer type</param>
        void NotifyConsumed(TimeSpan duration, string consumerType);

        /// <summary>
        /// Notify that a fault occurred during message consumption
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="consumerType"></param>
        /// <param name="exception"></param>
        void NotifyFaulted(TimeSpan duration, string consumerType, Exception exception);
    }
}