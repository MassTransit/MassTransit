// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Internal
{
	using System;
	using System.Threading;
	using Events;
	using Common.Logging;
	using Magnum.Extensions;
	using Magnum.Pipeline;

	public class ThreadPoolConsumerPool :
		ConsumerPool
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ThreadPoolConsumerPool));

		private readonly IServiceBus _bus;
		private readonly Pipe _eventAggregator;
		private readonly IObjectBuilder _objectBuilder;
		private int _consumerCount;
		private bool _disposed;
		private bool _enabled;
		private int _maximumThreadCount = 25;
		private int _receiverCount;
		private ISubscriptionScope _scope;
		private readonly TimeSpan _receiveTimeout;
		private readonly object _locker = new object();

		public ThreadPoolConsumerPool(IServiceBus bus, IObjectBuilder objectBuilder, Pipe eventAggregator, TimeSpan receiveTimeout)
		{
			_objectBuilder = objectBuilder;
			_receiveTimeout = receiveTimeout;
			_eventAggregator = eventAggregator;
			_bus = bus;
		}

		public int MaximumConsumerCount
		{
			get { return _maximumThreadCount; }
			set
			{
				if (value <= 0)
					throw new InvalidOperationException("The maximum thread count must be at least one");

				_maximumThreadCount = value;
			}
		}

		public void Start()
		{
			_scope = _eventAggregator.NewSubscriptionScope();
			_scope.Subscribe<ReceiveCompleted>(Handle);
			_scope.Subscribe<ConsumeCompleted>(Handle);

			_enabled = true;

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Starting Consumer Pool for {0}", _bus.Endpoint.Uri);

			QueueReceiver();
		}

		public void Stop()
		{
			_enabled = false;

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Stopping Consumer Pool for {0}", _bus.Endpoint.Uri);

			if(_consumerCount == 0)
				return;

			var completed = new AutoResetEvent(true);

			_scope.Subscribe<ConsumeCompleted>(x => completed.Set());

			while (completed.WaitOne(60.Seconds(), true))
			{
				if (_log.IsDebugEnabled)
					_log.DebugFormat("Consumer Pool stopped for {0}", _bus.Endpoint.Uri);

				if (_consumerCount == 0)
					return;
			}

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Timeout stopping Consumer Pool for {0}", _bus.Endpoint.Uri);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_scope != null)
				{
					_scope.Dispose();
					_scope = null;
				}
			}

			_disposed = true;
		}

		private void Handle(ReceiveCompleted message)
		{
			Interlocked.Decrement(ref _receiverCount);

			QueueReceiver();
		}

		private void Handle(ConsumeCompleted message)
		{
			Interlocked.Decrement(ref _consumerCount);

			QueueReceiver();
		}

		private void QueueReceiver()
		{
			if (_enabled == false)
				return;

			lock (_locker)
			{
				if (_receiverCount > 0)
					return;

				if (_consumerCount >= _maximumThreadCount)
					return;

				if (_log.IsDebugEnabled)
					_log.DebugFormat("Queueing receiver for {0}", _bus.Endpoint.Uri);

				var context = new ServiceBusReceiveContext(_bus, _objectBuilder, _eventAggregator, _receiveTimeout);

				Interlocked.Increment(ref _receiverCount);
				Interlocked.Increment(ref _consumerCount);

				try
				{
					ThreadPool.QueueUserWorkItem(x => context.ReceiveFromEndpoint());
				}
				catch (Exception ex)
				{
					_log.Error("Unable to queue consumer to thread pool", ex);

					Interlocked.Decrement(ref _receiverCount);
					Interlocked.Decrement(ref _consumerCount);
				}
			}

			_eventAggregator.Send(new ReceiverQueued
				{
					ReceiverCount = _receiverCount,
					ConsumerCount = _consumerCount,
				});
		}

		~ThreadPoolConsumerPool()
		{
			Dispose(false);
		}
	}
}