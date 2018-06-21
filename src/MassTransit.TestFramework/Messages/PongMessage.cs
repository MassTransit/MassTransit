// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.TestFramework.Messages
{
    using System;


    [Serializable]
	public class PongMessage :
		IEquatable<PongMessage>,
		CorrelatedBy<Guid>
	{
		private Guid _id;

		public PongMessage()
		{
			_id = Guid.NewGuid();
		}

		public PongMessage(Guid correlationId)
		{
			_id = correlationId;
		}

		public Guid CorrelationId
		{
			get { return _id; }
            set { _id = value; }
		}

		public bool Equals(PongMessage obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj._id.Equals(_id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (PongMessage)) return false;
			return Equals((PongMessage) obj);
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}
	}
}