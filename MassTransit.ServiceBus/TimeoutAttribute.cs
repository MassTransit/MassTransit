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

	[AttributeUsage(AttributeTargets.Class)]
	public class TimeoutAttribute :
		Attribute
	{
		private TimeSpan _timeout;

		public TimeoutAttribute()
		{
			_timeout = TimeSpan.MaxValue;
		}

		public int Seconds
		{
			set
			{
				if (_timeout == TimeSpan.MaxValue)
					_timeout = TimeSpan.FromSeconds(value);
				else
					_timeout += TimeSpan.FromSeconds(value);
			}
			get { return _timeout.Seconds; }
		}

		public int Minutes
		{
			set
			{
				if (_timeout == TimeSpan.MaxValue)
					_timeout = TimeSpan.FromMinutes(value);
				else
					_timeout += TimeSpan.FromMinutes(value);
			}
			get { return _timeout.Minutes; }
		}

		public TimeSpan Timeout
		{
			get { return _timeout; }
		}
	}
}