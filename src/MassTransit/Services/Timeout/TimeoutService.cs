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
namespace MassTransit.Services.Timeout
{
	using System;
	using System.Linq;
	using Exceptions;
	using log4net;
	using Magnum.Fibers;
	using Messages;
	using Saga;
	using Server;

	public class TimeoutService :
		IDisposable,
		Consumes<TimeoutScheduled>.All,
		Consumes<TimeoutRescheduled>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (TimeoutService));
		private readonly Fiber _queue = new ThreadPoolFiber();
		private readonly Scheduler _scheduler = new TimerScheduler(new ThreadPoolFiber());
		private IServiceBus _bus;
		private UnsubscribeAction _unsubscribeToken;
		private readonly ISagaRepository<TimeoutSaga> _repository;
		private ScheduledAction _unschedule;

		public TimeoutService(IServiceBus bus, ISagaRepository<TimeoutSaga> repository)
		{
			_bus = bus;
			_repository = repository;
		}

		public void Dispose()
		{
			try
			{
				_scheduler.Stop();
				_queue.Stop();

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

			_queue.Add(CheckExistingTimeouts);

			if (_log.IsInfoEnabled)
				_log.Info("Timeout Service Started");
		}

		public void Stop()
		{
			if (_log.IsInfoEnabled)
				_log.Info("Timeout Service Stopping");

			_queue.Stop();

			_unsubscribeToken();

			if (_log.IsInfoEnabled)
				_log.Info("Timeout Service Stopped");
		}

		private void CheckExistingTimeouts()
		{
			DateTime now = DateTime.UtcNow;

			_log.DebugFormat("TimeoutService Checking For Existing Timeouts: {0}", now.ToLocalTime());
			try
			{
				var sagas = _repository.Where(x => x.TimeoutAt < now && x.CurrentState == TimeoutSaga.WaitingForTime).ToArray();
				foreach (TimeoutSaga saga in sagas)
				{
					TimeoutSaga instance = saga;

					_queue.Add(() => _bus.Publish(new TimeoutExpired { CorrelationId = instance.CorrelationId, Tag = instance.Tag }));
				}
			}
			catch (Exception ex)
			{
				_log.Error("Error rescheduling existing timeouts", ex);
			}
			finally
			{
				if (_unschedule != null)
					_unschedule.Cancel();

				_log.DebugFormat("Scheduling next check at " + DateTime.Now);
				_unschedule = _scheduler.Schedule(1000, new ThreadPoolFiber(), ()=> _queue.Add(CheckExistingTimeouts));
			}
		}

		public void Consume(TimeoutScheduled message)
		{
			_queue.Add(CheckExistingTimeouts);
		}

		public void Consume(TimeoutRescheduled message)
		{
			_queue.Add(CheckExistingTimeouts);
		}
	}
}