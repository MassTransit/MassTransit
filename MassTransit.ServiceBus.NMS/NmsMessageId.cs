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
namespace MassTransit.ServiceBus.NMS
{
	using System;
	using Internal;

	public class NmsMessageId :
		IMessageId
	{
		private static readonly NmsMessageId _empty = new NmsMessageId(Guid.Empty, 0, 0, 0);

		private int _a;
		private int _b;
		private int _c;
		private Guid _id;

		protected NmsMessageId(Guid id, int a, int b, int c)
		{
			Assign(id, a, b, c);
		}

		public NmsMessageId()
		{
			Assign(_empty);
		}

		public NmsMessageId(NmsMessageId other)
		{
			Assign(other);
		}

		public NmsMessageId(string s)
			: this()
		{
			string[] parts = s.Split(':');
			if (parts.Length == 4)
			{
				Assign(new Guid(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
			}
			else
			{
				throw new Exception(string.Format("MessageId is in the wrong format. Should be like '00000000-0000-0000-0000-000000000000:0:0:0' not '{0}'", s));
			}
		}

		public NmsMessageId(IMessageId messageId)
		{
			NmsMessageId other = messageId as NmsMessageId;
			if (other == null)
				throw new Exception(string.Format("Unable to create an NMS MessageId from {0}", messageId));

			Assign(other);
		}

		public Guid Id
		{
			get { return _id; }
		}

		public static NmsMessageId Empty
		{
			get { return _empty; }
		}

		public int A
		{
			get { return _a; }
		}

		public int B
		{
			get { return _b; }
		}

		public int C
		{
			get { return _c; }
		}

		#region IMessageId Members

		public bool IsEmpty
		{
			get { return _empty.Equals(this); }
		}

		public bool Equals(IMessageId obj)
		{
			NmsMessageId other = obj as NmsMessageId;

			return Equals(other);
		}

		#endregion

		private void Assign(Guid id, int a, int b, int c)
		{
			_id = id;
			_a = a;
			_b = b;
			_c = c;
		}

		private void Assign(NmsMessageId other)
		{
			_id = other._id;
			_a = other.A;
			_b = other.B;
			_c = other.C;
		}

		public bool Equals(NmsMessageId other)
		{
			if (other == null)
				return false;

			if (_id != other._id)
				return false;
			if (_a != other._a)
				return false;
			if (_b != other._b)
				return false;
			if (_c != other._c)
				return false;

			return true;
		}

		public override string ToString()
		{
			return string.Format(@"{0}:{1}:{2}:{3}", _id, _a, _b, _c);
		}

		public override bool Equals(object obj)
		{
			if (obj is NmsMessageId)
				return Equals((NmsMessageId) obj);

			return false;
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode() + 37*_a + 29*_b + 11*_c;
		}

		public static implicit operator NmsMessageId(string id)
		{
			NmsMessageId result = Empty;

			if (!string.IsNullOrEmpty(id))
				result = new NmsMessageId(id);

			return result;
		}

		public static implicit operator string(NmsMessageId id)
		{
			return id.ToString();
		}
	}
}