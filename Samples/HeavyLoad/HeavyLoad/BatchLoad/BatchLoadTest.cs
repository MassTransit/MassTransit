namespace HeavyLoad.BatchLoad
{
	using System;
	using System.Threading;
	using log4net;
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.Internal;
	using MassTransit.ServiceBus.Threading;

	public class BatchLoadTest : IDisposable
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (BatchLoadTest));
		private IServiceBus _bus;
		private BatchLoadTestContainer _container;
		private ManagedThreadPool<int> _threads;

		public void Dispose()
		{
			_threads.Dispose();
			_bus.Dispose();
			_container.Dispose();
		}

		public void Run(StopWatch watch)
		{
			MsmqHelper.ValidateAndPurgeQueue(@".\private$\test_servicebus");

			_container = new BatchLoadTestContainer();

			_bus = _container.Resolve<IServiceBus>();

			watch.Start();

			//BatchServiceComponent component = new BatchServiceComponent();

			//_bus.Subscribe(component);

			_container.AddComponent<BatchServiceComponent>();

			_bus.AddComponent<BatchServiceComponent>();

			int batchCount = 100;

			CheckPoint publishCheckpoint = watch.Mark("Publishing " + batchCount + " batches");

			_threads = new ManagedThreadPool<int>(delegate(int batchLength)
			                                                                  	{
		                                                                  			SendBatch(batchLength);
			                                                                  	}, 10, 10);

			for (int i = 0; i < batchCount; i++)
			{
				_threads.Enqueue(100);
			}

			for (int i = 0; i < batchCount; i++)
			{
				bool received = BatchServiceComponent.Received.WaitOne(TimeSpan.FromSeconds(30), true);
				if (!received)
				{
					_log.Error("Something went wrong, we never got a batch completed");
					break;
				}
			}

			publishCheckpoint.Complete(batchCount * 100);

			watch.Stop();

			_log.Info("Test Complete");

		}

		private void SendBatch(int batchLength)
		{
			Guid batchId = Guid.NewGuid();

			for (int i = 0; i < batchLength; i++)
			{
				_bus.Publish(new BasicMessage(batchId, batchLength));
			}
		}
	}

	public class BatchServiceComponent : Consumes<Batch<BasicMessage, Guid>>.Selected
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (BatchServiceComponent));
		private static readonly Semaphore _received = new Semaphore(0, 1000);

		public static Semaphore Received
		{
			get { return _received; }
		}

		public void Consume(Batch<BasicMessage, Guid> message)
		{
			_log.Debug("Receiving batch: " + message.BatchId);

			int messageCount = 0;
			foreach (BasicMessage basicMessage in message)
			{
				if(basicMessage.BatchId != message.BatchId)
					_log.ErrorFormat("Mismatched batch id for batch {0} -- {1}", message.BatchId, basicMessage.BatchId);

				messageCount++;
			}

			if (messageCount == message.BatchLength)
				_log.Info("Batch complete: " + message.BatchId + ", Count = " + messageCount);
			else
				_log.ErrorFormat("Batch not received: {0}, Expected: {1}, Received: {2}", message.BatchId, message.BatchLength, messageCount);

			_received.Release();
		}

		public bool Accept(Batch<BasicMessage, Guid> message)
		{
			return true;
		}
	}

	[Serializable]
	public class BasicMessage : BatchedBy<Guid>
	{
		private readonly Guid _batchId;
		private readonly int _batchLength;

		public BasicMessage(Guid batchId, int batchLength)
		{
			_batchId = batchId;
			_batchLength = batchLength;
		}

		public Guid BatchId
		{
			get { return _batchId; }
		}

		public int BatchLength
		{
			get { return _batchLength; }
		}
	}
}