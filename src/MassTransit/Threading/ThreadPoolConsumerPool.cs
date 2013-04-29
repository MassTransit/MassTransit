// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Threading
{
	using System;
	using System.Threading;
	using Context;
	using Events;
	using Logging;
	using Magnum.Extensions;
	using Stact;

	/// <summary>
	/// The default consumer pool that enqueues consumes on the thread pool.
	/// </summary>
	public class ThreadPoolConsumerPool :
		ConsumerPool
	{
		static readonly ILog _log = Logger.Get(typeof (ThreadPoolConsumerPool));

		readonly IServiceBus _bus;
		readonly UntypedChannel _eventChannel;
		readonly object _locker = new object();
		readonly TimeSpan _receiveTimeout;
		int _consumerCount;
		bool _disposed;
		bool _enabled;
		ChannelConnection _eventConnection;
		int _maximumThreadCount = 25;
		int _receiverCount;

		public ThreadPoolConsumerPool(IServiceBus bus, UntypedChannel eventChannel, TimeSpan receiveTimeout)
		{
			_receiveTimeout = receiveTimeout;
			_bus = bus;
			_eventChannel = eventChannel;
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

		/// <summary>
		/// <para>Start the receiving thread pool.</para>
		/// 
		/// <para>This method implements a message-based receive loop;
		/// so instead of having a while-loop that receives non-stop, the event channel
		/// ensures that receives are enqueued whenever <see cref="ReceiveCompleted"/> is published
		/// in the event channel passed to this pool instance at construction time.</para>
		/// </summary>
		public void Start()
		{
			_eventConnection = _eventChannel.Connect(x =>
				{
					x.AddConsumerOf<ReceiveCompleted>()
						.UsingConsumer(Handle)
						.HandleOnCallingThread();

					x.AddConsumerOf<ConsumeCompleted>()
						.UsingConsumer(Handle)
						.HandleOnCallingThread();
				});

			_enabled = true;

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Starting Consumer Pool for {0}", _bus.Endpoint.Address.Uri);

			QueueReceiver();
		}

		public void Stop()
		{
			_enabled = false;

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Stopping Consumer Pool for {0}", _bus.Endpoint.Address.Uri);

			if (_consumerCount == 0)
				return;

			var completed = new AutoResetEvent(true);

			using (_eventChannel.Connect(x =>
				{
					x.AddConsumerOf<ConsumeCompleted>()
						.UsingConsumer(message => completed.Set())
						.HandleOnCallingThread();
				}))
			{
				while (completed.WaitOne(_bus.ShutdownTimeout, true))
				{
					if (_log.IsDebugEnabled)
						_log.DebugFormat("Consumer Pool stopped for {0}", _bus.Endpoint.Address.Uri);

					if (_consumerCount == 0)
						return;
				}
			}

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Timeout stopping Consumer Pool for {0}", _bus.Endpoint.Address.Uri);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_eventConnection != null)
				{
					_eventConnection.Dispose();
					_eventConnection = null;
				}
			}

			_disposed = true;
		}

		void Handle(ReceiveCompleted message)
		{
			Interlocked.Decrement(ref _receiverCount);

			QueueReceiver();
		}

		void Handle(ConsumeCompleted message)
		{
			Interlocked.Decrement(ref _consumerCount);

			QueueReceiver();
		}

		void QueueReceiver()
		{
			if (_enabled == false)
				return;

			lock (_locker)
			{
				if (_receiverCount > 0)
					return;

				if (_consumerCount >= _maximumThreadCount)
					return;

//				if (_log.IsDebugEnabled)
//					_log.DebugFormat("Queueing receiver for {0}", _bus.Endpoint.Address.Uri);

				var context = new ServiceBusReceiveContext(_bus, _eventChannel, _receiveTimeout);

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

			_eventChannel.Send(new ReceiverQueued
				{
					ReceiverCount = _receiverCount,
					ConsumerCount = _consumerCount,
				});
		}
	}
}