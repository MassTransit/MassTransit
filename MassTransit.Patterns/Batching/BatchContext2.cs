
namespace MassTransit.Patterns.Batching
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;
	using MassTransit.ServiceBus;

	public class BatchContext2<T, K> : 
		IBatchContext<T, K> where T : IBatchMessage
	{
		private readonly K _batchId;
		private readonly IServiceBus _bus;
		private readonly ManualResetEvent _complete = new ManualResetEvent(false);
		private readonly Queue<T> _messages = new Queue<T>();
		private readonly IEndpoint _returnEndpoint;
		private readonly TimeSpan _timeout;
		private int _messageCount;

		public BatchContext2(IMessageContext<T> context, K batchId, TimeSpan timeout)
		{
			_batchId = batchId;
			_timeout = timeout;
			_bus = context.Bus;
			_returnEndpoint = context.Envelope.ReturnEndpoint;
		    _messageCount = context.Message.BatchLength;
		}

		#region IBatchContext<T,K> Members

		public K BatchId
		{
			get { return _batchId; }
		}

		public IEndpoint ReturnEndpoint
		{
			get { return _returnEndpoint; }
		}

		public IServiceBus Bus
		{
			get { return _bus; }
		}

		public bool IsComplete
		{
			get { return _messageCount.Equals(_messages.Count); }
		}

	    public IEnumerable<T> Messages
	    {
            get
            {
                return _messages;    
            }
	        
	    }

        public void Enqueue(T message)
        {
            _messages.Enqueue(message);
        }

        //hmmm
	    IEnumerator<T> IEnumerable<T>.GetEnumerator()
	    {
	        return _messages.GetEnumerator();
	    }

	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        return _messages.GetEnumerator();
	    }

	    #endregion
	}
}