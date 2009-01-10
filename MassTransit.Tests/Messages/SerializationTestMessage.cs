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
namespace MassTransit.Tests.Messages
{
	using System;

	[Serializable]
	public class SerializationTestMessage :
		IEquatable<SerializationTestMessage>
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Count { get; set; }
		public long BigCount { get; set; }
		public decimal Amount { get; set; }
		public double Radians { get; set; }
		public DateTime Created { get; set; }

		public bool Equals(SerializationTestMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.Id.Equals(Id) &&
			       Equals(obj.Name, Name) &&
			       obj.Count == Count &&
			       obj.BigCount == BigCount &&
			       obj.Amount == Amount &&
			       obj.Radians == Radians &&
			       obj.Created.Equals(Created);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (SerializationTestMessage)) return false;
			return Equals((SerializationTestMessage) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = Id.GetHashCode();
				result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
				result = (result*397) ^ Count;
				result = (result*397) ^ BigCount.GetHashCode();
				result = (result*397) ^ Amount.GetHashCode();
				result = (result*397) ^ Radians.GetHashCode();
				result = (result*397) ^ Created.GetHashCode();
				return result;
			}
		}
	}
}