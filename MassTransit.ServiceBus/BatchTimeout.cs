namespace MassTransit.ServiceBus
{
	public class BatchTimeout<TMessage, TBatchId>
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
	}
}