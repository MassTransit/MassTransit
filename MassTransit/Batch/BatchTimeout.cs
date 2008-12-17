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
	using System;

	[Serializable]
	public class BatchTimeout<TMessage, TBatchId> :
		IEquatable<BatchTimeout<TMessage, TBatchId>>
		where TMessage : class, BatchedBy<TBatchId>
	{
		private readonly TBatchId _batchId;

		public BatchTimeout(TBatchId batchId)
		{
			_batchId = batchId;
		}

		public TBatchId BatchId
		{
			get { return _batchId; }
		}

		public bool Equals(BatchTimeout<TMessage, TBatchId> obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj._batchId, _batchId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (BatchTimeout<TMessage, TBatchId>)) return false;
			return Equals((BatchTimeout<TMessage, TBatchId>) obj);
		}

		public override int GetHashCode()
		{
			return _batchId.GetHashCode();
		}
	}
}