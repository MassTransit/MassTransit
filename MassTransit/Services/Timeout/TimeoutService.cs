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
    using System.Collections.Generic;
    using System.Threading;
    using Exceptions;
    using log4net;
    using Messages;
    using Util;

    public class TimeoutService :
		IDisposable
    {
        private static readonly TimeSpan _interval = TimeSpan.FromSeconds(1);
        private static readonly ILog _log = LogManager.GetLogger(typeof (TimeoutService));
        private IServiceBus _bus;
        private readonly ITimeoutRepository _repository;
        private readonly ManualResetEvent _stopped = new ManualResetEvent(false);
        private readonly AutoResetEvent _trigger = new AutoResetEvent(true);
        private Thread _watchThread;
    	private UnsubscribeAction _unsubscribeToken;

    	public TimeoutService(ITimeoutRepository repository)
    	{
    		_repository = repository;
    	}

    	public void Dispose()
        {
            try
            {
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

        public void Start(IServiceBus bus)
        {
			if (_log.IsInfoEnabled)
                _log.Info("Timeout Service Starting");

            _bus = bus;

        	_unsubscribeToken = _bus.Subscribe<ScheduleTimeoutConsumer>();
        	_unsubscribeToken += _bus.Subscribe<CancelTimeoutConsumer>();

            _repository.TimeoutAdded += TriggerPublisher;

            _watchThread = new Thread(PublishPendingTimeoutMessages);
            _watchThread.IsBackground = true;
            _watchThread.Start();

            if (_log.IsInfoEnabled)
                _log.Info("Timeout Service Started");
        }

        private void TriggerPublisher(Guid obj)
        {
            _trigger.Set();
        }

        public void Stop()
        {
            if (_log.IsInfoEnabled)
                _log.Info("Timeout Service Stopping");

        	_unsubscribeToken();

            if (_log.IsInfoEnabled)
                _log.Info("Timeout Service Stopped");
        }

        private void PublishPendingTimeoutMessages()
        {
            try
            {
                WaitHandle[] handles = new WaitHandle[] {_trigger, _stopped};

                while ((WaitHandle.WaitAny(handles, _interval, true)) != 1)
                {
                    DateTime lessThan = DateTime.UtcNow;

                    IList<Tuple<Guid, DateTime>> list = _repository.List(lessThan);
                    foreach (Tuple<Guid, DateTime> tuple in list)
                    {
                    	_log.InfoFormat("Publishing timeout message for {0}", tuple.Key);

                        _bus.Publish(new TimeoutExpired(tuple.Key));

                        _repository.Remove(tuple.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Unable to publish timeout message", ex);
            }
        }
    }
}