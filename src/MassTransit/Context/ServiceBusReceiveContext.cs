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
namespace MassTransit.Context
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using Events;
	using Exceptions;
	using Logging;
	using Magnum;
	using Stact;

	public class ServiceBusReceiveContext
	{
		static readonly ILog _log = Logger.Get(typeof (ServiceBusReceiveContext));

		readonly IServiceBus _bus;
		readonly Stopwatch _consumeTime;
		readonly UntypedChannel _eventChannel;
		readonly Stopwatch _receiveTime;
		readonly TimeSpan _receiveTimeout;
		int _consumeCount;
		bool _consumeNotified;
		IEnumerator<Action<IConsumeContext>> _consumers;
		bool _receiveNotified;
		DateTime _startTime;

		public ServiceBusReceiveContext(IServiceBus bus, UntypedChannel eventChannel, TimeSpan receiveTimeout)
		{
			_bus = bus;
			_eventChannel = eventChannel;
			_receiveTimeout = receiveTimeout;
			_receiveTime = new Stopwatch();
			_consumeTime = new Stopwatch();
			_consumeCount = 0;
		}

		public void ReceiveFromEndpoint()
		{
			try
			{
				_startTime = SystemUtil.UtcNow;
				_receiveTime.Start();

				// let the endpoint (and hence inbound transport) consume a message
				// The lambda passed is not called until the transport decides that it has
				// gotten a message and want to pass it forward.
				_bus.Endpoint.Receive(context =>
					{
						context.SetBus(_bus);
						
						// look inside the inbound pipeline and find all message sinks that match the receive
						// context (i.e. the message type we actually got from the transport)
						// Have a look at everything implementing IPipelineSink<IConsumeContext> to
						// dig deeper
						IEnumerable<Action<IConsumeContext>> enumerable = _bus.InboundPipeline.Enumerate(context);

						_consumers = enumerable.GetEnumerator();
						if (!_consumers.MoveNext())
						{
							_consumers.Dispose();
							return null; // meaning we don't have any sinks interested in this msg context
						}

						// otherwise, we have some consumers
						return DeliverMessageToConsumers;
					}, _receiveTimeout);
			}
			catch (ObjectDisposedException ex)
			{
				_log.Warn("Endpoint threw ObjectDisposedException", ex);
				Thread.Sleep(1000);
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

		void DeliverMessageToConsumers(IReceiveContext context)
		{
			try
			{
				NotifyReceiveCompleted();

				_receiveTime.Stop();
				_consumeTime.Start();

				if (_log.IsDebugEnabled)
					_log.DebugFormat("Dispatching message on {0} from thread {1}", _bus.Endpoint.Address.Uri,
						Thread.CurrentThread.ManagedThreadId);

				bool atLeastOneConsumerFailed = false;

				Exception lastException = null;

				do
				{
					try
					{
						_consumers.Current(context);
						_consumeCount++;
					}
					catch (Exception ex)
					{
						_log.Error(string.Format("'{0}' threw an exception consuming message '{1}'",
							_consumers.Current.GetType().FullName,
							context.GetType().FullName), ex);

						atLeastOneConsumerFailed = true;
						lastException = ex;
					}
				} while (_consumers.MoveNext());

				if (atLeastOneConsumerFailed)
				{
					throw new MessageException(context.GetType(),
						"At least one consumer threw an exception",
						lastException);
				}
			}
			finally
			{
				_consumeTime.Stop();

				_consumers.Dispose();
				_consumers = null;

				ReportConsumerTime(_startTime, _receiveTime.Elapsed, _consumeTime.Elapsed, context);
				ReportConsumerCount(context, _consumeCount);
			}
		}

		void ReportConsumerTime(DateTime startTime, TimeSpan receiveDuration, TimeSpan consumeDuration,
		                        IReceiveContext context)
		{
			var message = new MessageReceived
				{
					Context = context,
					ReceivedAt = startTime,
					ReceiveDuration = receiveDuration,
					ConsumeDuration = consumeDuration,
				};

			_eventChannel.Send(message);
		}

		void ReportConsumerCount(IConsumeContext context, int count)
		{
			var message = new MessageConsumed
				{
					Context = context,
					ConsumeCount = count,
				};

			_eventChannel.Send(message);
		}

		void NotifyReceiveCompleted()
		{
			if (_receiveNotified)
				return;

			_eventChannel.Send(new ReceiveCompleted());
			_receiveNotified = true;
		}

		void NotifyConsumeCompleted()
		{
			if (_consumeNotified)
				return;

			_eventChannel.Send(new ConsumeCompleted());
			_consumeNotified = true;
		}
	}
}