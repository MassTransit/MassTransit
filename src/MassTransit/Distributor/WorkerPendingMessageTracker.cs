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
namespace MassTransit.Distributor
{
	using System;
	using System.Collections.Generic;
	using Magnum.Threading;

	/// <summary>
	/// Keeps track of messages that were not accepted due to worker load in order to provide a pending message count
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class WorkerPendingMessageTracker<T> :
		IPendingMessageTracker<T>
	{
		readonly ReaderWriterLockedObject<HashSet<T>> _messages = new ReaderWriterLockedObject<HashSet<T>>(new HashSet<T>());

		bool _disposed;

		public void Viewed(T item)
		{
			_messages.UpgradeableReadLock(x =>
				{
					if (!x.Contains(item))
					{
						_messages.WriteLock(y => y.Add(item));
					}
				});
		}

		public void Dispose()
		{
			Dispose(true);
		}

		public void Consumed(T item)
		{
			_messages.UpgradeableReadLock(x =>
				{
					if (x.Contains(item))
					{
						_messages.WriteLock(y => y.Remove(item));
					}
				});
		}

		public int PendingMessageCount()
		{
			return _messages.ReadLock(x => x.Count);
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_messages.Dispose();
			}

			_disposed = true;
		}
	}
}