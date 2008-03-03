namespace MassTransit.Patterns.Batching
{
	using System;
	using System.Collections.Generic;
	using ServiceBus;

    [Serializable]
	public class BatchMessage<TMessageType, TBatchId> :
		IBatchMessage
	{
		private readonly TBatchId _batchId;
		private readonly int _batchLength;
		private TMessageType _body;

		public BatchMessage(TBatchId batchId, int batchLength, TMessageType body)
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

		public TMessageType Body
		{
			get { return _body; }
			set { _body = value; }
        }


        #region System.Object Overrides
        public override bool Equals(object obj)
        {
            BatchMessage<TMessageType, TBatchId> msg = obj as BatchMessage<TMessageType,TBatchId>;
            return msg != null &&
                   msg.BatchId.Equals(this.BatchId) &&
                   msg.BatchLength.Equals(this.BatchLength) &&
                   msg.Body.Equals(this.Body);
        }

        public override int GetHashCode()
        {
            return this.BatchId.GetHashCode() + this.BatchLength.GetHashCode() + this.Body.GetHashCode();
        }
        #endregion

        //just an idea at this point
        public static void SendAsBatch(IServiceBus bus, Uri endpoint, IList<TMessageType> messages)
        {
            Guid id = Guid.NewGuid();
            int count = messages.Count;

            foreach(TMessageType msg in messages)
            {
                bus.Send(null, new BatchMessage<TMessageType, Guid>(id, count, msg));
            }
        }

        public static void PublishAsBatch(IServiceBus bus, IList<TMessageType> messages)
        {
            Guid id = Guid.NewGuid();
            int count = messages.Count;

            foreach (TMessageType msg in messages)
            {
                bus.Publish(new BatchMessage<TMessageType, Guid>(id, count, msg));
            }
        }
	}
}