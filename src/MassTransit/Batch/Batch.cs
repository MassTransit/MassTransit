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
namespace MassTransit.Batch
{
	using System.Collections;
	using System.Collections.Generic;
	using Common.Logging;

	public class Batch<TMessage, TBatchId> :
		BatchedBy<TBatchId>,
		IEnumerable<TMessage>
		where TMessage : class, BatchedBy<TBatchId>
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(Batch<TMessage, TBatchId>));

		private readonly TBatchId _batchId;
		private readonly int _batchLength;
		private readonly IEnumerable<TMessage> _enumerable;

		public Batch(TBatchId batchId, int batchLength, IEnumerable<TMessage> enumerable)
		{
			_batchId = batchId;
			_batchLength = batchLength;
			_enumerable = enumerable;
		}

		public TBatchId BatchId
		{
			get { return _batchId; }
		}

		public int BatchLength
		{
			get { return _batchLength; }
		}

		IEnumerator<TMessage> IEnumerable<TMessage>.GetEnumerator()
		{
			foreach (TMessage message in _enumerable)
			{
				yield return message;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<TMessage>)this).GetEnumerator();
		}
	}
}