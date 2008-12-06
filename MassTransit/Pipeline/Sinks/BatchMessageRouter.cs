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
namespace MassTransit.Pipeline.Sinks
{
	using System;
	using System.Collections.Generic;
	using Magnum.Common.DateTimeExtensions;
	using Magnum.Common.Threading;

	/// <summary>
	/// Dispatches batch messages to the appropriate consumers
	/// </summary>
	/// <typeparam name="TMessage">The type of the message to be routed</typeparam>
	/// <typeparam name="TBatchId">The type of the batch id</typeparam>
	public class BatchMessageRouter<TMessage, TBatchId> :
		MessageRouterBase<TMessage, TBatchId>
		where TMessage : class, BatchedBy<TBatchId>
	{
		private ReaderWriterLockedObject<List<IMessageSink<BatchMessage<TMessage, TBatchId>>>> _consumerSinks;
		private bool _disposed;

		public BatchMessageRouter()
		{
			_consumerSinks = new ReaderWriterLockedObject<List<IMessageSink<BatchMessage<TMessage, TBatchId>>>>(new List<IMessageSink<BatchMessage<TMessage, TBatchId>>>());
		}

		public override IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			IMessageSink<TMessage> sink = null;

			_sinks.UpgradeableReadLock(x =>
				{
					if (x.TryGetValue(message.BatchId, out sink) == false)
					{
						_sinks.WriteLock(y =>
							{
								if (x.TryGetValue(message.BatchId, out sink) == false)
								{
									var batchMessage = new BatchMessage<TMessage, TBatchId>(message.BatchId, message.BatchLength, null);

									// we need to create a sink for this batch and get it wired up
									MessageRouter<TMessage> router = new MessageRouter<TMessage>();
									foreach (var messageSink in _consumerSinks.ReadLock(z => z))
									{
										foreach (var consumes in messageSink.Enumerate(batchMessage))
										{
											router.Connect(new BatchCombiner<TMessage, TBatchId>(message.BatchId, message.BatchLength, consumes));
										}
									}

									x.Add(message.BatchId, router);

									sink = router;
								}
							});
					}
				});

			if (sink == null)
				yield break;

			int consumerCount = 0;
			foreach (Consumes<TMessage>.All consumer in sink.Enumerate(message))
			{
				yield return consumer;
				consumerCount++;
			}

//			if(consumerCount == 0)
//			{
//				_sinks.WriteLock(x => x.Remove(message.BatchId));
//			}
		}

		public Func<bool> Connect(IMessageSink<BatchMessage<TMessage, TBatchId>> sink)
		{
			_consumerSinks.WriteLock(sinks => sinks.Add(sink));

			return () => _consumerSinks.WriteLock(sinks => sinks.Remove(sink));
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (_disposed) return;
			if (disposing)
			{
				_consumerSinks.Dispose();
				_consumerSinks = null;
			}

			_disposed = true;
		}

		~BatchMessageRouter()
		{
			Dispose(false);
		}
	}
}