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

	public class ReceivedMessagesList<T> :
		ReceivedMessages<T>,
		IDisposable
		where T : class
	{
		readonly IList<T> _messages;
		readonly ManualResetEvent _received;
		TimeSpan _timeout = 8.Seconds();

		public ReceivedMessagesList()
		{
			_received = new ManualResetEvent(false);
			_messages = new List<T>(1);
		}

		public void Dispose()
		{
			using (_received)
			{
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _messages.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Any()
		{
			while (_messages.Any() == false)
			{
				if (_received.WaitOne(_timeout, true) == false)
					return false;
			}

			return true;
		}

		public void Add(T message)
		{
			_messages.Add(message);
			_received.Set();
		}
	}
}