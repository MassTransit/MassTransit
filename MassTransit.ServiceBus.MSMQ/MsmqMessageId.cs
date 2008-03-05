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
namespace MassTransit.ServiceBus.MSMQ
{
	using System;
	using Internal;

	public class MsmqMessageId :
		IMessageId
	{
		private static readonly MsmqMessageId _empty = new MsmqMessageId(Guid.Empty, 0);

		private readonly Guid _id;
		private readonly int _sequence;

		protected MsmqMessageId(Guid id, int sequence)
		{
			_id = id;
			_sequence = sequence;
		}

		public MsmqMessageId()
		{
			_id = _empty.Id;
			_sequence = _empty.Sequence;
		}

		public MsmqMessageId(MsmqMessageId other)
		{
			_id = other._id;
			_sequence = other._sequence;
		}

		public MsmqMessageId(string s)
			: this()
		{
			string[] parts = s.Split('\\');
			if (parts.Length == 2)
			{
				_id = new Guid(parts[0]);
				_sequence = int.Parse(parts[1]);
			}
			else
			{
				throw new Exception(string.Format("MessageId is in the wrong format. Should be like '00000000-0000-0000-0000-000000000000\\0' not '{0}'", s));
			}
		}

		public MsmqMessageId(IMessageId messageId)
		{
			MsmqMessageId other = messageId as MsmqMessageId;
			if (other == null)
				throw new Exception(string.Format("Unable to create an MSMQ MessageId from {0}", messageId));

			_id = other.Id;
			_sequence = other.Sequence;
		}

		public Guid Id
		{
			get { return _id; }
		}

		public int Sequence
		{
			get { return _sequence; }
		}

		public static MsmqMessageId Empty
		{
			get { return _empty; }
		}

		#region IMessageId Members

		public bool IsEmpty
		{
			get { return _empty.Equals(this); }
		}

		public bool Equals(IMessageId obj)
		{
			MsmqMessageId other = obj as MsmqMessageId;

			return Equals(other);
		}

		#endregion

		public bool Equals(MsmqMessageId other)
		{
			if (other == null)
				return false;

			if (_id != other._id)
				return false;

			if (_sequence != other._sequence)
				return false;

			return true;
		}

		public override string ToString()
		{
			return string.Format(@"{0}\{1}", _id, _sequence);
		}

		public override bool Equals(object obj)
		{
			if (obj is MsmqMessageId)
				return Equals((MsmqMessageId) obj);

			return false;
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode() + 29*_sequence.GetHashCode();
		}

		public static implicit operator MsmqMessageId(string id)
		{
			MsmqMessageId result = Empty;

			if (!string.IsNullOrEmpty(id))
				result = new MsmqMessageId(id);

			return result;
		}

		public static implicit operator string(MsmqMessageId id)
		{
			return id.ToString();
		}
	}
}