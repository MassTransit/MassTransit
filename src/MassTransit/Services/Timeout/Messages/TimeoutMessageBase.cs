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
namespace MassTransit.Services.Timeout.Messages
{
	using System;

	[Serializable]
	public class TimeoutMessageBase
	{
		/// <summary>
		/// The tag associated with the timeout message
		/// This is mainly because we can't publish type-specific messages yet. 
		/// We really want to be able to schedule a timeout and specify a message type
		/// to publish on the timeout, but that is going to be tough to handle (period).
		/// </summary>
		public int Tag { get; set; }

		/// <summary>
		/// The time (in UTC) when the timeout expires
		/// </summary>
		public DateTime TimeoutAt { get; set; }

		/// <summary>
		/// The CorrelationId to use when publishing the timeout message
		/// </summary>
		public Guid CorrelationId { get; set; }
	}
}