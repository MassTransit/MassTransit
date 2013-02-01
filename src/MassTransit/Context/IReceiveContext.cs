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
    using System.Collections.Generic;
    using System.IO;
    using Context;
    using Serialization;
    using Util;


    /// <summary>
    /// Receive context that allows receiving sinks to 
    /// </summary>
    public interface IReceiveContext :
        IConsumeContext
    {
        Stream BodyStream { get; }
        IEnumerable<ISent> Sent { get; }

        IEnumerable<IReceived> Received { get; }

        Guid Id { get; }

        /// <summary>
        /// True if the transport is transactional and will leave the message on the queue if an exception is thrown
        /// </summary>
        bool IsTransactional { get; }

        /// <summary>
        ///  The original message id that was consumed
        /// </summary>
        string OriginalMessageId { get; }

        /// <summary>
        /// Set the content type that was indicated by the transport message header
        /// </summary>
        /// <param name="value"></param>
        void SetContentType(string value);

        void SetMessageId(string value);

        void SetInputAddress(Uri uri);

        void SetEndpoint(IEndpoint endpoint);

        /// <summary>
        /// Sets the bus which is receiving this message/data.
        /// </summary>
        /// <param name="bus">Bus instance</param>
        void SetBus([NotNull] IServiceBus bus);

        void SetRequestId(string value);

        void SetConversationId(string value);

        void SetCorrelationId(string value);

        void SetOriginalMessageId(string value);

        void SetSourceAddress(Uri uri);

        void SetDestinationAddress(Uri uri);

        void SetResponseAddress(Uri uri);

        void SetFaultAddress(Uri uri);

        void SetNetwork(string value);

        void SetRetryCount(int retryCount);

        void SetExpirationTime(DateTime value);

        void SetMessageType(string messageType);

        void SetHeader(string key, string value);

        /// <summary>
        /// Sets the context's body stream;
        /// useful for wrapped serializers 
        /// such as encrypting serializers
        /// and for testing.
        /// </summary>
        /// <param name="stream">Stream to replace the previous stream with</param>
        void SetBodyStream([NotNull] Stream stream);

        void CopyBodyTo(Stream stream);

        void SetMessageTypeConverter(IMessageTypeConverter messageTypeConverter);

        /// <summary>
        /// Notify that a fault needs to be sent, so that the endpoint can send it when
        /// appropriate.
        /// </summary>
        /// <param name="faultAction"></param>
        void NotifyFault(Action faultAction);

        void NotifySend(ISendContext context, IEndpointAddress address);

        void NotifySend<T>(ISendContext<T> sendContext, IEndpointAddress address)
            where T : class;

        void NotifyPublish<T>(IPublishContext<T> publishContext)
            where T : class;

        void NotifyConsume<T>(IConsumeContext<T> consumeContext, string consumerType, string correlationId)
            where T : class;

        /// <summary>
        /// Publish any pending faults for the context
        /// </summary>
        void ExecuteFaultActions(IEnumerable<Action> faultActions);

        /// <summary>
        /// Returns the fault actions that were added to the context during message processing
        /// </summary>
        /// <returns></returns>
        IEnumerable<Action> GetFaultActions();
    }
}