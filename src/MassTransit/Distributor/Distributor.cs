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
namespace MassTransit.Distributor
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Logging;
	using Magnum;
	using Magnum.Extensions;
	using Messages;
	using Stact;
	using Stact.Internal;

	public class Distributor<TMessage> :
		IDistributor<TMessage>
		where TMessage : class
	{
		readonly int _pingTimeout = (int) 5.Seconds().TotalMilliseconds;
		readonly IWorkerSelectionStrategy<TMessage> _selectionStrategy;

		readonly IDictionary<Uri,WorkerDetails> _workers = new Dictionary<Uri,WorkerDetails>();

		Fiber _fiber;
		ScheduledOperation _scheduled;
		Scheduler _scheduler;
		UnsubscribeAction _unsubscribeAction = () => false;
		IServiceBus _bus;

		public Distributor(IWorkerSelectionStrategy<TMessage> workerSelectionStrategy)
		{
			_selectionStrategy = workerSelectionStrategy;

			_fiber = new PoolFiber();
			_scheduler = new TimerScheduler(new PoolFiber());
		}

		public Distributor()
			: this(new DefaultWorkerSelectionStrategy<TMessage>())
		{
		}

		public void Consume(TMessage message)
		{
			WorkerDetails worker;
			lock (_workers)
			{
				worker = _selectionStrategy.SelectWorker(_workers.Values, message);
				if (worker == null)
				{
					_bus.MessageContext<TMessage>().RetryLater();
					return;
				}

				worker.Add();
			}

			IEndpoint endpoint = _bus.GetEndpoint(worker.DataUri);

			var distributed = new Distributed<TMessage>(message, _bus.Context().ResponseAddress);

			endpoint.Send(distributed);
		}

		public bool Accept(TMessage message)
		{
			lock(_workers)
				return _selectionStrategy.HasAvailableWorker(_workers.Values, message);
		}

		bool _disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Distributor()
		{
			Dispose(false);
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_scheduler.Stop(60.Seconds());
				_scheduler = null;

				_fiber.Shutdown(60.Seconds());
				_fiber = null;
			}

			_disposed = true;
		}
		public void Start(IServiceBus bus)
		{
			_bus = bus;

			_unsubscribeAction = bus.SubscribeHandler<WorkerAvailable<TMessage>>(Consume);

			// don't plan to unsubscribe this since it's an important thing
			bus.SubscribeInstance(this);

			_scheduled = _scheduler.Schedule(_pingTimeout, _pingTimeout, _fiber, PingWorkers);

			_bus.Publish(new PingWorker());
		}

		public void Stop()
		{
			_scheduled.Cancel();
			_scheduled = null;

			lock(_workers)
				_workers.Clear();

			_unsubscribeAction();
		}

		void Consume(WorkerAvailable<TMessage> message)
		{
			WorkerDetails worker;
			lock (_workers)
			{
				worker = _workers.Retrieve(message.ControlUri, () =>
					{
						return new WorkerDetails
							{
								ControlUri = message.ControlUri,
								DataUri = message.DataUri,
								InProgress = message.InProgress,
								InProgressLimit = message.InProgressLimit,
								Pending = message.Pending,
								PendingLimit = message.PendingLimit,
								LastUpdate = message.Updated,
							};
					});
			}

			worker.UpdateInProgress(message.InProgress, message.InProgressLimit, message.Pending, message.PendingLimit,
				message.Updated);

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Worker {0}: {1} in progress, {2} pending", message.DataUri, message.InProgress, message.Pending);
		}

		static readonly ILog _log = Logger.Get(typeof (Distributor<TMessage>));

		void PingWorkers()
		{
			List<WorkerDetails> expired;
			lock(_workers)
			{
				expired = _workers.Values
					.Where(x => x.LastUpdate < SystemUtil.UtcNow.Subtract(_pingTimeout.Milliseconds()))
					.ToList();
			}

			expired.ForEach(x =>
					{
						_bus.GetEndpoint(x.ControlUri).Send(new PingWorker(),
							context => context.SetResponseAddress(_bus.Endpoint.Address.Uri));
					});
		}
	}
}