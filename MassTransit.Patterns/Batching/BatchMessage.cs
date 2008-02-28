namespace MassTransit.Patterns.Batching
{
	using System;

	[Serializable]
	public class BatchMessage<T, K> :
		IBatchMessage
	{
		private readonly K _batchId;
		private readonly int _batchLength;
		private T _body;

		public BatchMessage(K batchId, int batchLength, T body)
		{
			_body = body;
			_batchId = batchId;
			_batchLength = batchLength;
		}

		/// <summary>
		/// The number of messages in the batch
		/// </summary>
		public int BatchLength
		{
			get { return _batchLength; }
		}

		public object BatchId
		{
			get { return _batchId; }
		}

		public T Body
		{
			get { return _body; }
			set { _body = value; }
		}
	}
}