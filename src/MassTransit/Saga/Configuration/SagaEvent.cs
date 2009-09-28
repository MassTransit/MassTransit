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
namespace MassTransit.Saga.Configuration
{
	using System;
	using Magnum.StateMachine;

	public class SagaEvent<T>
		where T : SagaStateMachine<T>, ISaga
	{
		public Event Event { get; set; }
		public Type MessageType { get; set; }

		public SagaEvent(Event eevent, Type messageType)
		{
			Event = eevent;
			MessageType = messageType;
		}

		public bool Equals(SagaEvent<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Event, Event) && Equals(other.MessageType, MessageType);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (SagaEvent<T>)) return false;
			return Equals((SagaEvent<T>) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Event != null ? Event.GetHashCode() : 0)*397) ^ (MessageType != null ? MessageType.GetHashCode() : 0);
			}
		}
	}
}