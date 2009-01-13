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
namespace HeavyLoad.BatchLoad
{
	using System;
	using System.Threading;
	using Castle.Windsor;
	using log4net;
	using MassTransit;
	using MassTransit.Batch;
	using MassTransit.Threading;
	using MassTransit.Transports.Msmq;

    public class BatchLoadTest :
		IDisposable
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (BatchLoadTest));
		private IServiceBus _bus;
		private IWindsorContainer _container;
		private ManagedThreadPool<int> _threads;

		public void Dispose()
		{
			_threads.Dispose();
			_bus.Dispose();
			_container.Dispose();
		}

		public void Run(StopWatch watch)
		{
			_container = new HeavyLoadContainer();

			_bus = _container.Resolve<IServiceBus>();

            MsmqEndpoint endpoint = _bus.Endpoint as MsmqEndpoint;
            if (endpoint != null)
                MsmqUtilities.ValidateAndPurgeQueue(endpoint.QueuePath);

			watch.Start();

			//BatchServiceComponent component = new BatchServiceComponent();

			//_bus.Subscribe(component);

			_container.AddComponent<BatchServiceComponent>();

			_bus.Subscribe<BatchServiceComponent>();

			int batchCount = 100;

			CheckPoint publishCheckpoint = watch.Mark("Publishing " + batchCount + " batches");

			_threads = new ManagedThreadPool<int>(delegate(int batchLength) { SendBatch(batchLength); }, 10, 10);

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

			publishCheckpoint.Complete(batchCount*100);

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

	public class BatchServiceComponent : 
		Consumes<Batch<BasicMessage, Guid>>.All
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
				if (basicMessage.BatchId != message.BatchId)
					_log.ErrorFormat("Mismatched batch id for batch {0} -- {1}", message.BatchId, basicMessage.BatchId);

				messageCount++;
			}

			if (messageCount == message.BatchLength)
				_log.Debug("Batch complete: " + message.BatchId + ", Count = " + messageCount);
			else
				_log.ErrorFormat("Batch not received: {0}, Expected: {1}, Received: {2}", message.BatchId, message.BatchLength, messageCount);

			_received.Release();
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