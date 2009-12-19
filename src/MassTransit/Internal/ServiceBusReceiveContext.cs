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
namespace MassTransit.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using Events;
	using Exceptions;
	using log4net;
	using Magnum.InterfaceExtensions;
	using Magnum.Pipeline;
	using Magnum.Reflection;

	public class ServiceBusReceiveContext
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ServiceBusReceiveContext));

		private readonly Stopwatch _receiveTime;
		private readonly Stopwatch _consumeTime;
		private readonly IServiceBus _bus;
		private readonly IObjectBuilder _objectBuilder;
		private readonly Pipe _eventAggregator;
		private IEnumerator<Action<object>> _consumers;
		private int _consumeCount;
		private bool _receiveNotified;
		private bool _consumeNotified;
		private readonly TimeSpan _receiveTimeout;

		public ServiceBusReceiveContext(IServiceBus bus, IObjectBuilder objectBuilder, Pipe eventAggregator, TimeSpan receiveTimeout)
		{
			_bus = bus;
			_receiveTimeout = receiveTimeout;
			_objectBuilder = objectBuilder;
			_eventAggregator = eventAggregator;
			_receiveTime = new Stopwatch();
			_consumeTime = new Stopwatch();
			_consumeCount = 0;
		}

		public void ReceiveFromEndpoint()
		{
			try
			{
				if(_log.IsDebugEnabled)
					_log.DebugFormat("Calling Receive on {0} from thread {1} ({2})", _bus.Endpoint.Uri, 
						Thread.CurrentThread.ManagedThreadId, _receiveTimeout);

				_receiveTime.Start();

				_bus.Endpoint.Receive(message =>
					{
						if (_log.IsDebugEnabled)
							_log.DebugFormat("Enumerating pipeline on {0} from thread {1}", _bus.Endpoint.Uri, 
								Thread.CurrentThread.ManagedThreadId);

						InboundMessageHeaders.SetCurrent(context =>
							{
								context.ReceivedOn(_bus);
								context.SetObjectBuilder(_objectBuilder);
								context.ReceivedAs(message);
							});

						IEnumerable<Action<object>> enumerable = _bus.InboundPipeline.Enumerate(message);

						_consumers = enumerable.GetEnumerator();
						if (!_consumers.MoveNext())
						{
							_consumers.Dispose();
							return null;
						}

						return DeliverMessageToConsumers;
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

		public void DeliverMessageToConsumers(object message)
		{
			try
			{
				NotifyReceiveCompleted();

				_receiveTime.Stop();
				_consumeTime.Start();

				if (_log.IsDebugEnabled)
					_log.DebugFormat("Dispatching message on {0} from thread {1}", _bus.Endpoint.Uri, Thread.CurrentThread.ManagedThreadId);

				bool atLeastOneConsumerFailed = false;

				Exception lastException = null;

				do
				{
					try
					{
						_consumers.Current(message);
						_consumeCount++;
					}
					catch (Exception ex)
					{
						_log.Error(string.Format("'{0}' threw an exception consuming message '{1}'",
							_consumers.Current.GetType().FullName,
							message.GetType().FullName), ex);

						atLeastOneConsumerFailed = true;
						lastException = ex;

						CreateAndPublishFault(message, ex);
					}
				} while (_consumers.MoveNext());

				if (atLeastOneConsumerFailed)
				{
					throw new MessageException(message.GetType(),
						"At least one consumer threw an exception",
						lastException);
				}
			}
			finally
			{
				_consumeTime.Stop();

				_consumers.Dispose();
				_consumers = null;

				ReportConsumerTime(message.GetType(), _receiveTime.Elapsed, _consumeTime.Elapsed);
				ReportConsumerCount(message.GetType(), _consumeCount);
			}
		}

		private void CreateAndPublishFault(object message, Exception ex)
		{
			if (message.Implements(typeof (CorrelatedBy<>)))
				this.FastInvoke("PublishFault", FastActivator.Create(typeof (Fault<,>), message, ex));
			else
				this.FastInvoke("PublishFault", FastActivator.Create(typeof (Fault<>), message, ex));
		}

		// this is called via reflection
		// ReSharper disable UnusedMember.Local
		private void PublishFault<T>(T message) where T : class
		// ReSharper restore UnusedMember.Local
		{
			CurrentMessage.GenerateFault(message);
		}

		private void ReportConsumerTime(Type messageType, TimeSpan receiveTime, TimeSpan consumeTime)
		{
			var message = new MessageReceived
				{
					MessageType = messageType,
					ReceiveDuration = receiveTime,
					ConsumeDuration = consumeTime,
				};

			_eventAggregator.Send(message);
		}

		private void ReportConsumerCount(Type messageType, int count)
		{
			var message = new MessageConsumed
				{
					MessageType = messageType,
					ConsumeCount = count,
				};

			_eventAggregator.Send(message);
		}

		private void NotifyReceiveCompleted()
		{
			if(_receiveNotified)
				return;

			_eventAggregator.Send(new ReceiveCompleted());
			_receiveNotified = true;
		}

		private void NotifyConsumeCompleted()
		{
			if(_consumeNotified)
				return;

			_eventAggregator.Send(new ConsumeCompleted());
			_consumeNotified = true;
		}
	}
}