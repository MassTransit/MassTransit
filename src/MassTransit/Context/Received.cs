// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
	public class Received<T> :
		IReceived
		where T : class
	{
		readonly IConsumeContext<T> _context;
		string _consumerType;
		long _timestamp;

		public Received(IConsumeContext<T> context, string consumerType, long timestamp)
		{
			_timestamp = timestamp;
			_consumerType = consumerType;
			_context = context;
		}

		public long Timestamp
		{
			get { return _timestamp; }
		}

		public string MessageType
		{
			get { return typeof (T).ToMessageName(); }
		}

		public string ReceiverType
		{
			get { return _consumerType; }
		}
	}
}