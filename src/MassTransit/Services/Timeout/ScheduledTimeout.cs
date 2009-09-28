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
namespace MassTransit.Services.Timeout
{
	using System;

	public class ScheduledTimeout
	{
		public virtual Guid Id { get; set; }
		public virtual int Tag { get; set; }
		public virtual DateTime ExpiresAt { get; set; }

		public virtual bool Equals(ScheduledTimeout obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.Id.Equals(Id) && obj.Tag == Tag;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (ScheduledTimeout)) return false;
			return Equals((ScheduledTimeout) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id.GetHashCode()*397) ^ Tag;
			}
		}
	}
}