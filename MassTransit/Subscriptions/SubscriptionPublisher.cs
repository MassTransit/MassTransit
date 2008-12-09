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
namespace MassTransit.Subscriptions
{
	using System;
	using Magnum.Common.Threading;
	using Messages;
	using Pipeline;

	public class SubscriptionPublisher :
		IDisposable
	{
		private readonly IServiceBus _bus;
		private volatile bool _disposed;
		private ReaderWriterLockedObject<SubscriptionSnapshot> _snapshot;

		public SubscriptionPublisher(IServiceBus bus)
		{
			_bus = bus;

			_snapshot = new ReaderWriterLockedObject<SubscriptionSnapshot>(new SubscriptionSnapshot());
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Refresh(MessagePipeline pipeline)
		{
			if (_disposed) throw new ObjectDisposedException("The publisher has been disposed");

			SubscriptionCollector collector = new SubscriptionCollector();

			_snapshot.WriteLock(x =>
				{
					pipeline.Inspect(collector);

					x.UpdateFromSnapshot(collector.Snapshot, Added, Removed);
				});
		}

		private void Added(Type messageType, string correlationId)
		{
			_bus.Publish(new AddSubscription(new Subscription(messageType, correlationId, _bus.Endpoint.Uri)));
		}

		private void Removed(Type messageType, string correlationId)
		{
			_bus.Publish(new RemoveSubscription(new Subscription(messageType, correlationId, _bus.Endpoint.Uri)));
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_snapshot.Dispose();
				_snapshot = null;
			}

			_disposed = true;
		}

		~SubscriptionPublisher()
		{
			Dispose(false);
		}
	}
}