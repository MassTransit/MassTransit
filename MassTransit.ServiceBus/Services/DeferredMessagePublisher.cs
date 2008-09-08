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
namespace MassTransit.Services
{
	using log4net;
	using ServiceBus;

	public class DeferredMessagePublisher :
		IHostedService
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (DeferredMessagePublisher));
		private readonly IServiceBus _bus;

		public DeferredMessagePublisher(IServiceBus bus)
		{
			_bus = bus;
		}

		public void Dispose()
		{
		}

		public void Start()
		{
			if (_log.IsInfoEnabled)
				_log.Info("Deferred Message Publisher Starting");

			_bus.AddComponent<DeferMessageConsumer>();
			_bus.AddComponent<TimeoutExpiredConsumer>();

			if (_log.IsInfoEnabled)
				_log.Info("Deferred Message Publisher Started");
		}

		public void Stop()
		{
			if (_log.IsInfoEnabled)
				_log.Info("Deferred Message Publisher Stopping");

			_bus.RemoveComponent<TimeoutExpiredConsumer>();
			_bus.RemoveComponent<DeferMessageConsumer>();

			if (_log.IsInfoEnabled)
				_log.Info("Deferred Message Publisher Stopped");
		}
	}
}