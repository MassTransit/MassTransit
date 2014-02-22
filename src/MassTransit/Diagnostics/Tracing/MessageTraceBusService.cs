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
namespace MassTransit.Diagnostics.Tracing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using Events;
    using Logging;
    using Magnum.Collections;
    using Magnum.Extensions;
    using Stact;

    public class MessageTraceBusService :
		IBusService,
		Consumes<GetMessageTraceList>.All
	{
		static readonly ILog _log = Logger.Get(typeof (MessageTraceBusService));
		readonly ChannelConnection _connection;
		readonly UntypedChannel _eventChannel;

		readonly Deque<ReceivedMessageTraceDetail> _messages;

		IServiceBus _controlBus;
		readonly int _detailLimit;
		readonly Fiber _fiber;
		UnsubscribeAction _unsubscribe;

		public MessageTraceBusService(UntypedChannel eventChannel)
		{
			_eventChannel = eventChannel;
			_fiber = new PoolFiber();
			_messages = new Deque<ReceivedMessageTraceDetail>();
			_detailLimit = 100;

			_connection = _eventChannel.Connect(x =>
				{
					x.AddConsumerOf<MessageReceived>()
						.UsingConsumer(Handle)
						.HandleOnFiber(_fiber);
				});
		}

		public void Consume(GetMessageTraceList message)
		{
			IConsumeContext<GetMessageTraceList> context = ContextStorage.MessageContext<GetMessageTraceList>();

			Uri responseAddress = context.ResponseAddress;
			if (responseAddress == null)
				return;

			int count = message.Count;

			IEndpoint responseEndpoint = context.Bus.GetEndpoint(responseAddress);

			_fiber.Add(() => SendLastTraceMessagesTo(responseEndpoint, count));
		}

		public void Dispose()
		{
			_connection.Dispose();
		}

		public void Start(IServiceBus bus)
		{
			_controlBus = bus;
			_unsubscribe = _controlBus.SubscribeInstance(this);
		}

		public void Stop()
		{
			_unsubscribe();
			_connection.Disconnect();

			_fiber.Shutdown(30.Seconds());
		}

		void Handle(MessageReceived message)
		{
			DateTime startTime = message.ReceivedAt;

			var detail = new ReceivedMessageTraceDetailImpl
				{
					Id = message.Context.Id,
					MessageId = message.Context.MessageId,
					MessageType = message.Context.MessageType,
					ContentType = message.Context.ContentType,
					SourceAddress = message.Context.SourceAddress,
					InputAddress = message.Context.InputAddress,
					DestinationAddress = message.Context.DestinationAddress,
					ResponseAddress = message.Context.ResponseAddress,
					FaultAddress = message.Context.FaultAddress,
					Network = message.Context.Network,
					ExpirationTime = message.Context.ExpirationTime,
					RetryCount = message.Context.RetryCount,
					StartTime = message.ReceivedAt,
					Duration = message.ReceiveDuration,
				};

			detail.Receivers = message.Context.Received.Select(x => (ReceiverTraceDetail) new ReceiverTraceDetailImpl
				{
					MessageType = x.MessageType,
					ReceiverType = x.ReceiverType,
					CorrelationId = x.CorrelationId,
					StartTime = startTime + TimeSpan.FromMilliseconds(x.Timestamp),
					Duration = TimeSpan.Zero,
				}).ToList();

			detail.SentMessages = message.Context.Sent.Select(x => (SentMessageTraceDetail) new SentMessageTraceDetailImpl
				{
					Id = x.Context.Id,
					MessageId = x.Context.MessageId,
					MessageType = x.Context.MessageType,
					DeclaringMessageType = x.Context.DeclaringMessageType.ToShortTypeName(),
					ContentType = x.Context.ContentType,
					Address = x.Address.Uri,
					SourceAddress = x.Context.SourceAddress,
					InputAddress = x.Context.InputAddress,
					DestinationAddress = x.Context.DestinationAddress,
					ResponseAddress = x.Context.ResponseAddress,
					FaultAddress = x.Context.FaultAddress,
					Network = x.Context.Network,
					ExpirationTime = x.Context.ExpirationTime,
					RetryCount = x.Context.RetryCount,
					StartTime = startTime + TimeSpan.FromMilliseconds(x.Timestamp),
					Duration = TimeSpan.Zero,
				}).ToList();

			_messages.AddToBack(detail);

			RemoveExtraDetailFromList();
		}

		void RemoveExtraDetailFromList()
		{
			int removeCount = _messages.Count - _detailLimit;
			if (removeCount > 0)
			{
				_messages.RemoveRange(0, removeCount);
			}
		}

		void SendLastTraceMessagesTo(IEndpoint endpoint, int count)
		{
			try
			{
				IList<ReceivedMessageTraceDetail> details = _messages.Reverse().Take(count).ToList();

				var message = new ReceivedMessageTraceListImpl {Messages = details};

				endpoint.Send<ReceivedMessageTraceList>(message);
			}
			catch (Exception ex)
			{
				_log.Error("Failed to send message", ex);
			}
		}
	}
}