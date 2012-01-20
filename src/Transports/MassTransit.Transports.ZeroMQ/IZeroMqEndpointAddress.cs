// Copyright 2007-2011 Henrik Feldt
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
namespace MassTransit.Transports.ZeroMq
{
	using System;
	using ZMQ;

	public interface IZeroMqEndpointAddress : IEndpointAddress
	{
		string Host { get; }
		int Port { get; }
		Transport ZmqTransport { get; }

		/// <summary>
		/// 	Incoming signals
		/// </summary>
		Uri PullSocket { get; }

		/// <summary>
		/// 	Outgoing signals
		/// </summary>
		Uri PushSocket { get; }

		/// <summary>
		/// 	Incoming data by subscription.
		/// </summary>
		Uri SubSocket { get; }

		/// <summary>
		/// 	Outgoing data per subscription.
		/// </summary>
		Uri PubSocket { get; }

		/// <summary>
		/// 	Incoming socket for routing.
		/// </summary>
		Uri RouterSocket { get; }

		/// <summary>
		/// 	Outgoing fair-routing socket.
		/// </summary>
		Uri DealerSocket { get; }
	}
}