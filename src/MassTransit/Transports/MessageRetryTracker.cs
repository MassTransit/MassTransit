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
namespace MassTransit.Transports
{
	using System.Collections.Generic;
	using Magnum.Threading;

	public class MessageRetryTracker
	{
		private readonly int _retryLimit;

		private readonly ReaderWriterLockedObject<Dictionary<string, int>> _messages = new ReaderWriterLockedObject<Dictionary<string, int>>(new Dictionary<string, int>());

		public MessageRetryTracker(int retryLimit)
		{
			_retryLimit = retryLimit;
		}

		public bool IsRetryLimitExceeded(string id)
		{
			int retryCount = 0;
			if (!_messages.ReadLock(x => x.TryGetValue(id, out retryCount)))
				return false;

			return retryCount >= _retryLimit;
		}

		public void IncrementRetryCount(string id)
		{
			_messages.WriteLock(x => { x[id] = x.ContainsKey(id) ? x[id] + 1 : 1; });
		}

		public void MessageWasReceivedSuccessfully(string id)
		{
			_messages.WriteLock(x =>
				{
					if (x.ContainsKey(id))
						x.Remove(id);
				});
		}
	}
}