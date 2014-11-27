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
namespace MassTransit.Testing.Scenarios
{
	using System;

	/// <summary>
	/// A test scenario that allows the tester to
	/// get hold of what messages were published, skipped, sent and received.
	/// Inherits IDisposable.
	/// </summary>
	public interface TestScenario :
		IDisposable
	{
		/// <summary>
		/// Gets the input bus. This is the bus that has *incoming* messages, i.e. the bus
		/// that you receive messages from.
		/// </summary>
		IBus InputBus { get; }

		/// <summary>
		/// Gets the output bus. This is the bus that has *outgoing* messages, i.e. the bus
		/// that you publish and send on.
		/// </summary>
	    ISendEndpoint InputQueueSendEndpoint { get; }

		/// <summary>
		/// The list of published messages is contained within this instance.
		/// </summary>
		PublishedMessageList Published { get; }

		/// <summary>
		/// The list of received messages is contained within this instance.
		/// </summary>
		ReceivedMessageList Received { get; }

		/// <summary>
		/// The list of send messages is contained within this instance.
		/// </summary>
		SentMessageList Sent { get; }

		/// <summary>
		/// The list of skipped messages is contained within this instance.
		/// </summary>
		ReceivedMessageList Skipped { get; }
	}
}