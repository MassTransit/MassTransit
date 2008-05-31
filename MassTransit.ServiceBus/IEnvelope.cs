/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

namespace MassTransit.ServiceBus
{
	using System;
	using Internal;

	/// <summary>
	/// A public interface to the envelope containing message(s)
	/// </summary>
	/// <remarks>
	/// An interface on top of the underlying messaging infrastructure's 'messages'
	/// </remarks>
	public interface IEnvelope
	{
		/// <summary>
		/// The unique identifier of this envelope
		/// </summary>
		IMessageId Id { get; set; }

		/// <summary>
		/// The unique identifier of the original envelope this envelope is in response to
		/// </summary>
		IMessageId CorrelationId { get; set; }

		/// <summary>
		/// The return endpoint for the message(s) in the envelope
		/// </summary>
		IEndpoint ReturnEndpoint { get; }

		/// <summary>
		/// The messages contained in the envelope
		/// </summary>
		object Message { get; set; }

		/// <summary>
		/// The label stored on the envelope
		/// </summary>
		string Label { get; set; }

		/// <summary>
		/// Indicates whether the message should be delivered in a recoverable method
		/// </summary>
		bool Recoverable { get; set; }

		/// <summary>
		/// Specifies the time before the envelope is no longer valid and should be discarded
		/// </summary>
		TimeSpan TimeToBeReceived { get; set; }

		/// <summary>
		/// The time the envelope was sent (only valid for received envelopes)
		/// </summary>
		DateTime SentTime { get; set; }

		/// <summary>
		/// The time the envelope arrived (only valid for received envelopes)
		/// </summary>
		DateTime ArrivedTime { get; set; }
	}
}