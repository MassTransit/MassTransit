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
namespace MassTransit.Testing.Subjects
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Magnum.Extensions;

	public class ReceivedMessageList :
		IReceivedMessageList,
		IDisposable
	{
		readonly IList<IReceivedMessage> _messages;
		readonly ManualResetEvent _received;
		TimeSpan _timeout = 8.Seconds();

		public ReceivedMessageList()
		{
			_messages = new List<IReceivedMessage>();
			_received = new ManualResetEvent(false);
		}

		public void Dispose()
		{
			using (_received)
			{
			}
		}

		public IEnumerator<IReceivedMessage> GetEnumerator()
		{
			lock (_messages)
				return _messages.ToList().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Any()
		{
			bool any;
			lock (_messages)
				any = _messages.Any();

			while (any == false)
			{
				if (_received.WaitOne(_timeout, true) == false)
					return false;

				lock (_messages)
					any = _messages.Any();
			}

			return true;
		}

		public void Add(IReceivedMessage message)
		{
			lock (_messages)
				_messages.Add(message);

			_received.Set();
		}
	}
}