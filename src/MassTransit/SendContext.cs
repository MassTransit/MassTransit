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
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Transports;


    /// <summary>
    /// Unlike the old world, the send context is returned from the endpoint and used to configure the message sending.
    /// That way the message is captured by the endpoint and then any configuration is done at the higher level.
    /// </summary>
    public interface SendContext
    {
        Uri SourceAddress { get; set; }
        Uri DestinationAddress { get; set; }
        Uri ResponseAddress { get; set; }
        Uri FaultAddress { get; set; }

        Guid? RequestId { get; set; }
        Guid? MessageId { get; set; }
        Guid? CorrelationId { get; set; }

        TimeSpan? TimeToLive { get; set; }

        string ContentType { get; set; }

        /// <summary>
        /// True if the message should be persisted to disk to survive a broker restart
        /// </summary>
        bool Durable { get; set; }


        /// <summary>
        /// The serializer to use when serializing the message to the transport
        /// </summary>
        IMessageSendSerializer Serializer { get; set; }

        void Match<T>(Action<T> callback)
            where T : class, SendContext;
    }


    public interface Endpoint
    {
        Task<SendContext<T>> Send<T>(T message)
            where T : class;

        Task<SendContext> Send(object message, Type messageType);
    }

    public interface PublishConvention
    {
        IEndpoint GetDestinationEndpoint<T>(T message);
        IEndpoint GetDestinationEndpoint(object message, Type messageType);
    }

    public interface SendContext<T> :
        SendContext
        where T : class
    {
        T Message { get; }
    }
}