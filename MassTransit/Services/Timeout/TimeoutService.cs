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

	public class TimeoutService :
		IDisposable,
		Consumes<ScheduleTimeout>.All,
		Consumes<CancelTimeout>.All
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (TimeoutService));
		private readonly CommandQueue _queue = new ThreadPoolCommandQueue();
		private readonly ITimeoutRepository _repository;
		private readonly Scheduler _scheduler = new ThreadPoolScheduler();
		private IServiceBus _bus;
		private UnsubscribeAction _unsubscribeToken;

		public TimeoutService(IServiceBus bus, ITimeoutRepository repository)
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
				ScheduledTimeout soonest = _repository
					.OrderBy(x => x.ExpiresAt)
					.FirstOrDefault();

				if (soonest == null)
					return;

				DateTime now = DateTime.UtcNow;
				if (soonest.ExpiresAt < now)
					_queue.Enqueue(PublishPendingTimeoutMessages);
				else
				{
					TimeSpan interval = soonest.ExpiresAt - now;
					if (interval > 30.Seconds())
						_scheduler.Schedule(30000, CheckExistingTimeouts);
					else
					{
						_scheduler.Schedule((int) interval.TotalMilliseconds, PublishPendingTimeoutMessages);
					}
				}

			}
			catch (Exception ex)
			{
				_log.Error("Unable to retrieve existing timeouts", ex);
			}
		}

		public void Consume(ScheduleTimeout message)
		{
			try
			{
				var timeout = new ScheduledTimeout
					{
						Id = message.CorrelationId,
						Tag = message.Tag,
						ExpiresAt = message.TimeoutAt
					};

				_repository.Schedule(timeout);

				CheckExistingTimeouts();
			}
			catch (Exception ex)
			{
				_log.Error(string.Format("Unable to cancel a scheduled timeout {0}:{1}", message.CorrelationId, message.Tag), ex);
				throw;
			}
		}

		public void Consume(CancelTimeout message)
		{
			try
			{
				var timeout = new ScheduledTimeout
					{
						Id = message.CorrelationId,
						Tag = message.Tag
					};

				_repository.Remove(timeout);
			}
			catch (Exception ex)
			{
				_log.Error(string.Format("Unable to cancel a scheduled timeout {0}:{1}", message.CorrelationId, message.Tag), ex);
				throw;
			}
		}

		private void PublishPendingTimeoutMessages()
		{
			try
			{
				var now = DateTime.UtcNow;

				var expired = _repository
					.Where(x => x.ExpiresAt <= now)
					.OrderBy(x => x.ExpiresAt)
					.ToList();

				foreach (var timeout in expired)
				{
					try
					{
						_log.InfoFormat("Publishing timeout message for {0}:{1}", timeout.Id, timeout.Tag);

						_bus.Publish(new TimeoutExpired {CorrelationId = timeout.Id, Tag = timeout.Tag});

						_repository.Remove(timeout);
					}
					catch (Exception ex)
					{
						_log.Error("A problem occurred publishing the timeout expiration", ex);
					}
				}

				_queue.Enqueue(CheckExistingTimeouts);
			}
			catch (Exception ex)
			{
				_log.Error("Unable to retrieve a timeout from the repository", ex);
			}
		}
	}
}