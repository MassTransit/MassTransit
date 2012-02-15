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
namespace MassTransit.Transports
{
	using System;

	/// <summary>
	/// The inbound transport takes messages from the underlying transport technology and hands it to the
	/// Action{IReceiveContext} that can be gotten from the lookup function 
	/// passed to the <see cref="Receive"/> method.
	/// </summary>
	public interface IInboundTransport :
		ITransport
	{
		/// <summary>
		/// Implementors should take messages from the inbound transport technology,
		/// call the lookup function to get a callback that passes messages to all
		/// routed sinks for the context's message type and properties and then
		/// ACK the receive.
		/// </summary>
		/// <param name="lookupSinkChain">A lookup function that takes a receive context
		/// and gives back either a non-null action handler for </param>
		void Receive(Func<IReceiveContext, Action<IReceiveContext>> lookupSinkChain, TimeSpan timeout);
	}
}