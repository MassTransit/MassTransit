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
namespace MassTransit.Diagnostics.Tracing
{
    using System;

    public interface ReceiverTraceDetail
	{
		/// <summary>
		/// The message type consumed by the receiver
		/// </summary>
		string MessageType { get; }

		/// <summary>
		/// The type of receiver that consumed the message
		/// </summary>
		string ReceiverType { get; }

		/// <summary>
		/// Correlation information related to the receiver (saga Id, etc.)
		/// </summary>
		string CorrelationId { get; }

		/// <summary>
		/// The time the receiver started processing the message, relative to the receive time
		/// </summary>
		DateTime StartTime { get; }

		/// <summary>
		/// The time at which the receiver was done processing the message
		/// </summary>
		TimeSpan Duration { get; }
	}
}