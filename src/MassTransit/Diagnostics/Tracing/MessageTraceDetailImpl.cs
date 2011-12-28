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

    public class MessageTraceDetailImpl :
		MessageTraceDetail
	{
		/// <summary>
		/// The amount of time spent processing the message
		/// </summary>
		public TimeSpan Duration { get; set; }

		public string MessageId { get; set; }
		public string MessageType { get; set; }
		public string ContentType { get; set; }
		public Uri SourceAddress { get; set; }
		public Uri InputAddress { get; set; }
		public Uri DestinationAddress { get; set; }
		public Uri ResponseAddress { get; set; }
		public Uri FaultAddress { get; set; }
		public string Network { get; set; }
		public DateTime? ExpirationTime { get; set; }
		public int RetryCount { get; set; }

		/// <summary>
		/// The trace identifier for this message receive
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The date/time the message was received from the transport
		/// </summary>
		public DateTime StartTime { get; set; }
	}
}