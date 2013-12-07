// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.IO;

    public interface ISendContext<T> :
        ISendContext,
        IMessageContext<T>
        where T : class
    {
        /// <summary>
        /// Sets the message content type that is used to deserialize the message
        /// </summary>
        /// <param name="value"></param>
        void SetContentType(string value);

        /// <summary>
        /// Sets the writer for the message body
        /// </summary>
        /// <param name="bodyWriter"></param>
        void SetBodyWriter(Action<Stream> bodyWriter);

        /// <summary>
        /// Sets the receive context that the send was created in for tracing
        /// </summary>
        /// <param name="receiveContext"></param>
        void SetReceiveContext(IReceiveContext receiveContext);
    }

    public interface ISendContext :
        IMessageContext
    {
        /// <summary>
        /// The identifier for this message publish/send
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The original message type that was sent/published
        /// </summary>
        Type DeclaringMessageType { get; }

        string OriginalMessageId { get; }

        void SetMessageType(string messageType);

        void SetRequestId(string value);

        void SetConversationId(string value);

        void SetCorrelationId(string value);

        void SetSourceAddress(Uri uri);

        void SetDestinationAddress(Uri uri);

        void SetResponseAddress(Uri uri);

        void SetFaultAddress(Uri uri);

        void SetNetwork(string network);

        void SetExpirationTime(DateTime value);

        void SetRetryCount(int retryCount);

        void SetUsing(IMessageContext context);

        void SetHeader(string key, string value);

        /// <summary>
        /// Set the delivery mode of the message
        /// </summary>
        /// <param name="deliveryMode"></param>
        void SetDeliveryMode(DeliveryMode deliveryMode);

        DeliveryMode DeliveryMode { get; }

        /// <summary>
        /// Serializes the message to the stream
        /// </summary>
        /// <param name="stream">The target stream for the serialized message</param>
        void SerializeTo(Stream stream);

        /// <summary>
        /// Determines if the send context can be converted to the requested type
        /// </summary>
        /// <typeparam name="T">The requested type</typeparam>
        /// <param name="context">The resulting context that was created for the requested message type</param>
        /// <returns>True if the message can be assigned to the requested type, otherwise false</returns>
        bool TryGetContext<T>(out IBusPublishContext<T> context)
            where T : class;

        /// <summary>
        /// Called when the send context has been used to send a message to an endpoint
        /// </summary>
        /// <param name="address">The address to which the message was sent</param>
        void NotifySend(IEndpointAddress address);
    }
}