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
namespace MassTransit.ServiceBus.Timeout
{
	using System;
	using System.Threading;
	using Exceptions;
	using log4net;
	using Messages;

    public class TimeoutService :
		IHostedService
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (TimeoutService));
		private readonly IServiceBus _bus;
		private readonly ITimeoutStorage _storage;
		private Thread _watchThread;

		public TimeoutService(IServiceBus bus, ITimeoutStorage storage)
		{
			_bus = bus;
			_storage = storage;
		}

		public void Dispose()
		{
			try
			{
				_bus.Dispose();
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

			_storage.Start();

			_bus.AddComponent<ScheduleTimeoutConsumer>();
			_bus.AddComponent<CancelTimeoutConsumer>();

			_watchThread = new Thread(PublishPendingTimeoutMessages);
			_watchThread.IsBackground = true;
			_watchThread.Start();

			if (_log.IsInfoEnabled)
				_log.Info("Timeout Service Started");
		}

		public void Stop()
		{
			if (_log.IsInfoEnabled)
				_log.Info("Timeout Service Stopping");

			_storage.Stop();

			_bus.RemoveComponent<ScheduleTimeoutConsumer>();
            _bus.RemoveComponent<CancelTimeoutConsumer>();

			if (_log.IsInfoEnabled)
				_log.Info("Timeout Service Stopped");
		}

		private void PublishPendingTimeoutMessages()
		{
			try
			{
				foreach (Guid id in _storage)
				{
					_bus.Publish(new TimeoutExpired(id));
				}
			}
			catch (Exception ex)
			{
				_log.Error("Unable to publish timeout message", ex);
			}
		}
	}
}