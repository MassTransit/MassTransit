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

using MassTransit.Util;

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

	/// <summary>
	/// The context keeps track of some statistics about the consumption
	/// of the message. Both when the receive operation completes
	/// and when the consume operation completes, does this context
	/// broadcast that information on the passed <see cref="UntypedChannel"/>
	/// passed in the constructor.
	/// </summary>
    [DebuggerNonUserCode]
    public class ServiceBusReceiveContext
	{
		static readonly ILog _log = Logger.Get(typeof (ServiceBusReceiveContext));

		readonly IServiceBus _bus;

		readonly UntypedChannel _eventChannel;
		readonly TimeSpan _receiveTimeout;
		IEnumerator<Action<IConsumeContext>> _consumers;

		/// <summary>number of consumers that were selected for the receive context</summary>
		int _consumeCount;

		bool _consumeNotified;
		bool _receiveNotified;

		readonly Stopwatch _receiveTime;
		readonly Stopwatch _consumeTime;
		DateTime _startTime;

		/// <summary>
		/// Creates a new service bus receive context, from a bus
		/// and a channel which will be used to broadcast statistical
		/// information.
		/// </summary>
		/// <param name="bus">The Service Bus instance</param>
		/// <param name="eventChannel">The Event Channel to broadcast timing information on</param>
		/// <param name="receiveTimeout">The receive timeout</param>
		public ServiceBusReceiveContext(IServiceBus bus, UntypedChannel eventChannel, TimeSpan receiveTimeout)
		{
			_bus = bus;
			_eventChannel = eventChannel;
			_receiveTimeout = receiveTimeout;
			_receiveTime = new Stopwatch();
			_consumeTime = new Stopwatch();
			_consumeCount = 0;
		}

		/// <summary>
		/// <para>Performs a receive from the endpoint that is specified on the bus given in the 
		/// constructor. First try to do the receive, then let the endpoint/transport
		/// call <see cref="DeliverMessageToConsumers"/> (if there are consumers interested)</para>
		/// <para>This method must not throw exceptions, because it might not be run on the main thread.</para>
		/// </summary>
        [DebuggerNonUserCode]
        public void ReceiveFromEndpoint()
		{
			try
			{
				_startTime = SystemUtil.UtcNow;
				_receiveTime.Start();

				// Let the endpoint (and hence inbound transport) consume a message.
				// This lambda passed is not called until the transport decides that it has
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
							// meaning we don't have any sinks interested in this msg context
							_consumers.Dispose();
							return null; 
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

		/// <summary>
		/// <para>Deliver the message to the consumers selected in <see cref="ReceiveFromEndpoint"/>. Assumption:
		/// the inbound transport will send the same context to this method as it did
		/// to the lambda in <see cref="ReceiveFromEndpoint"/>.</para>
		/// <para>This method will try to give the message to all consumers found.</para>
		/// </summary>
		/// <param name="context">The receive context</param>
		/// <exception cref="MessageException">If at least one consumer throws
		/// an exception, then a MessageException will be thrown. If multiple consumers
		/// threw exceptions, then the last exception will be the inner exception
		/// and the others won't be tracked.</exception>
		[DebuggerNonUserCode]
		void DeliverMessageToConsumers([NotNull] IReceiveContext context)
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