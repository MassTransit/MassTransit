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
namespace MassTransit.Transports.Msmq
{
    using System;
    using System.Messaging;

    public interface IMsmqEndpointAddress :
		IEndpointAddress
	{
		/// <summary>
		/// The format name used to receive messages
		/// </summary>
		string InboundFormatName { get; }

        /// <summary>
        /// The URI of the inbound address
        /// </summary>
        Uri InboundUri { get; }

		/// <summary>
		/// The name of the queue in local format (.\private$\name)
		/// </summary>
		string LocalName { get; }

		/// <summary>
		/// If specified, the multicast address to bind to the queue
		/// </summary>
		string MulticastAddress { get; }

		/// <summary>
		/// The format name used to send messages (may be different if multicast MSMQ is used)
		/// </summary>
		string OutboundFormatName { get; }

        /// <summary>
        /// For non-transactional queues only, determines how to set <see cref="Message.Recoverable"/> property on messages destined for this queue.
        /// </summary>
        bool IsRecoverable { get; }
	}
}