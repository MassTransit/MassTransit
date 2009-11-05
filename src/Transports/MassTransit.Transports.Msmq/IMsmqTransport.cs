// Copyright 2007-2008 The Apache Software Foundation.
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

	/// <summary>
	/// A message-level interface to the MSMQ transport
	/// </summary>
	public interface IMsmqTransport :
		ITransport
	{
		IMsmqEndpointAddress MsmqAddress { get; }

		/// <summary>
		/// Receive a message from the transport
		/// </summary>
		/// <param name="receiver"></param>
		void Receive(Func<Message, Action<Message>> receiver);

		void Receive(Func<Message, Action<Message>> receiver, TimeSpan timeout);

		void Send(Action<Message> sender);
	}
}