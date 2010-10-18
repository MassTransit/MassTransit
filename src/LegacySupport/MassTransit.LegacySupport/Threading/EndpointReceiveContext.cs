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
namespace MassTransit.Services.LegacyProxy.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using Events;
    using log4net;
    using Magnum.Pipeline;

    public class EndpointReceiveContext
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (EndpointReceiveContext));

        readonly Stopwatch _receiveTime;
        readonly Stopwatch _consumeTime;
        readonly IEndpoint _bus;
        readonly IObjectBuilder _objectBuilder;
        readonly Pipe _eventAggregator;
        IEnumerator<Action<object>> _consumers;
        int _consumeCount;
        bool _receiveNotified;
        bool _consumeNotified;
        readonly TimeSpan _receiveTimeout;
        readonly Action<object> _workToDo;

        public EndpointReceiveContext(IEndpoint bus,Pipe eventAggregator, TimeSpan receiveTimeout, Action<object> workToDo)
		{
			_bus = bus;
			_receiveTimeout = receiveTimeout;
			_eventAggregator = eventAggregator;
			_receiveTime = new Stopwatch();
			_consumeTime = new Stopwatch();
			_consumeCount = 0;
            _workToDo = workToDo;
		}

        public void ReceiveFromEndpoint()
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Calling Receive on {0} from thread {1} ({2})", _bus.Uri,
                        Thread.CurrentThread.ManagedThreadId, _receiveTimeout);

                _receiveTime.Start();

                _bus.Receive(message =>
                {
                    return _workToDo;

                }, _receiveTimeout);
            }
            catch (Exception ex)
            {
                _log.Error("Consumer Exception Exposed", ex);
            }
            finally
            {
                NotifyReceiveCompleted();
                NotifyConsumeCompleted();
            }
        }

        private void NotifyReceiveCompleted()
        {
            if (_receiveNotified)
                return;

            _eventAggregator.Send(new ReceiveCompleted());
            _receiveNotified = true;
        }

        private void NotifyConsumeCompleted()
        {
            if (_consumeNotified)
                return;

            _eventAggregator.Send(new ConsumeCompleted());
            _consumeNotified = true;
        }
    }
}