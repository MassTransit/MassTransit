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
	using Util;

	/// <summary>
	/// Envelopes are abstractions that represent an exchange between an endpoint and a client and/or service
	/// </summary>
	public class Envelope :
		IEnvelope, IEquatable<Envelope>
	{
		private DateTime _arrivedTime;
		private MessageId _correlationId = MessageId.Empty;
		private MessageId _id = MessageId.Empty;
		private string _label;
		private IMessage[] _messages;
		private bool _recoverable;
		private IEndpoint _returnEndpoint;
		private DateTime _sentTime;
		private TimeSpan _timeToBeReceived = TimeSpan.MaxValue;

		/// <summary>
		/// Initializes an envelope
		/// </summary>
		/// <param name="returnEndpoint">The return address for the envelope</param>
		/// <param name="messages">The messages stored in the envelope</param>
		public Envelope(IEndpoint returnEndpoint, params IMessage[] messages)
		{
			_returnEndpoint = returnEndpoint;
			_messages = messages;
		}

		/// <summary>
		/// Initializes an envelope
		/// </summary>
		/// <param name="messages">The messages stored in the envelope</param>
		public Envelope(params IMessage[] messages)
		{
			_messages = messages;
		}

		#region IEnvelope Members

		/// <summary>
		/// The unique identifier of this envelope
		/// </summary>
		public MessageId Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		/// The unique identifier of the original envelope this envelope is in response to
		/// </summary>
		public MessageId CorrelationId
		{
			get { return _correlationId; }
			set { _correlationId = value; }
		}

		/// <summary>
		/// The return address for the envelope
		/// </summary>
		public IEndpoint ReturnEndpoint
		{
			get { return _returnEndpoint; }
			set { _returnEndpoint = value; }
		}

		/// <summary>
		/// The messages contained in the envelope
		/// </summary>
		public IMessage[] Messages
		{
			get { return _messages ?? new IMessage[0]; }
			set { _messages = value; }
		}

		/// <summary>
		/// The label stored on the envelope
		/// </summary>
		public string Label
		{
			get { return _label; }
			set { _label = value; }
		}

		/// <summary>
		/// Indicates whether the message should be delivered in a recoverable method
		/// </summary>
		public bool Recoverable
		{
			get { return _recoverable; }
			set { _recoverable = value; }
		}

		/// <summary>
		/// Specifies the time before the envelope is no longer valid and should be discarded
		/// </summary>
		public TimeSpan TimeToBeReceived
		{
			get { return _timeToBeReceived; }
			set { _timeToBeReceived = value; }
		}

		/// <summary>
		/// The time the envelope was sent (only valid for received envelopes)
		/// </summary>
		public DateTime SentTime
		{
			get { return _sentTime; }
			set { _sentTime = value; }
		}

		/// <summary>
		/// The time the envelope arrived (only valid for received envelopes)
		/// </summary>
		public DateTime ArrivedTime
		{
			get { return _arrivedTime; }
			set { _arrivedTime = value; }
		}

		///<summary>
		///Creates a new object that is a copy of the current instance.
		///</summary>
		///
		///<returns>
		///A new object that is a copy of this instance.
		///</returns>
		///<filterpriority>2</filterpriority>
		public object Clone()
		{
			Envelope env = new Envelope(ReturnEndpoint, Messages);
			env.ArrivedTime = ArrivedTime;
			env.Id = Id;
			env.CorrelationId = CorrelationId;
			env.Label = Label;
			env.Recoverable = Recoverable;
			env.SentTime = SentTime;
			env.TimeToBeReceived = TimeToBeReceived;

			return env;
		}

		#endregion

		#region IEquatable<Envelope> Members

		///<summary>
		///Indicates whether the current object is equal to another object of the same type.
		///</summary>
		/// <param name="envelope">The envelope to compare this one against</param>
		///<returns>
		///true if the current object is equal to the other parameter; otherwise, false.
		///</returns>
		public bool Equals(Envelope envelope)
		{
			if (envelope == null) return false;
			return Equals(_id, envelope._id);
		}

		#endregion

		///<summary>
		///Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
		///</summary>
		///
		///<returns>
		///true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
		///</returns>
		///
		///<param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as Envelope);
		}

		///<summary>
		///Serves as a hash function for a particular type. 
		///</summary>
		///
		///<returns>
		///A hash code for the current <see cref="T:System.Object"></see>.
		///</returns>
		///<filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}
	}
}