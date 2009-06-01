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
namespace MassTransit.Services.Timeout
{
	using System;
	using System.Linq;
	using Exceptions;
	using log4net;
	using Magnum.Actors;
	using Magnum.Actors.CommandQueues;
	using Magnum.Actors.Schedulers;
	using Magnum.DateTimeExtensions;
	using Messages;
	using Saga;
	using Server;

	public class TimeoutService :
		IDisposable,
		Consumes<TimeoutScheduled>.All,
		Consumes<TimeoutRescheduled>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (TimeoutService));
		private readonly CommandQueue _queue = new ThreadPoolCommandQueue();
		private readonly Scheduler _scheduler = new ThreadPoolScheduler();
		private IServiceBus _bus;
		private UnsubscribeAction _unsubscribeToken;
		private ISagaRepository<TimeoutSaga> _repository;

		public TimeoutService(IServiceBus bus, ISagaRepository<TimeoutSaga> repository)
		{
			_bus = bus;
			_repository = repository;
		}

		public void Dispose()
		{
			try
			{
				_scheduler.Dispose();
				_queue.Disable();

				_bus.Dispose();
				_bus = null;
			}
			catch (Exception ex)
			{
				string message = "Error in shutting down the TimeoutService: " + ex.Message;
				ShutDownException exp = new ShutDownException(message, ex);
				_log.Error(message, exp);
				throw exp;
			}
		}

		public void Start()
		{
			if (_log.IsInfoEnabled)
				_log.Info("Timeout Service Starting");

			_unsubscribeToken = _bus.Subscribe(this);
			_unsubscribeToken += _bus.Subscribe<TimeoutSaga>();

			_queue.Enqueue(CheckExistingTimeouts);

			if (_log.IsInfoEnabled)
				_log.Info("Timeout Service Started");
		}

		public void Stop()
		{
			if (_log.IsInfoEnabled)
				_log.Info("Timeout Service Stopping");

			_queue.Disable();

			_unsubscribeToken();

			if (_log.IsInfoEnabled)
				_log.Info("Timeout Service Stopped");
		}

		private void CheckExistingTimeouts()
		{
			try
			{
				DateTime now = DateTime.UtcNow;

				var sagas = _repository.Where(x => x.TimeoutAt < now && x.CurrentState == TimeoutSaga.WaitingForTime).ToArray();
				foreach (TimeoutSaga saga in sagas)
				{
					_bus.Publish(new TimeoutExpired { CorrelationId = saga.CorrelationId, Tag = saga.Tag });
				}

			}
			catch (Exception ex)
			{
				_log.Error("Error rescheduling existing timeouts", ex);
			}
			finally
			{
				_scheduler.Schedule(1000, CheckExistingTimeouts);
			}
		}

		public void Consume(TimeoutScheduled message)
		{
			_queue.Enqueue(CheckExistingTimeouts);
		}

		public void Consume(TimeoutRescheduled message)
		{
			_queue.Enqueue(CheckExistingTimeouts);
		}
	}
}