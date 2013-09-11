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
namespace BusDriver.Commands
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Formatting;
	using MassTransit.Diagnostics.Tracing;
	using MassTransit.Logging;
	using Magnum.Extensions;
	using MassTransit;

    public class TraceCommand :
		Consumes<ReceivedMessageTraceList>.All,
		Command,
		IPendingCommand
	{
		static readonly ILog _log = Logger.Get(typeof (TraceCommand));
		readonly ManualResetEvent _complete;
		readonly int _count;
		readonly string _uriString;
		UnsubscribeAction _unsubscribe;

		public TraceCommand(string uriString, int count)
		{
			_uriString = uriString;
			_count = count;
			_complete = new ManualResetEvent(false);
		}

		public bool Execute()
		{
			Uri uri = _uriString.ToUri("The trace URI was invalid");

		    IServiceBus bus = Program.GetBus(uri.Scheme);

			IEndpoint endpoint = bus.GetEndpoint(uri);

			_log.DebugFormat("Sending trace request to {0} for {1} message{2}", uri, _count, _count == 1 ? "" : "s");

			_unsubscribe = bus.SubscribeInstance(this);

			endpoint.Send<GetMessageTraceList>(new GetMessageTraceListImpl {Count = _count}, x => x.SendResponseTo(bus));

			Program.AddPendingCommand(this);

			return true;
		}

		public void Consume(ReceivedMessageTraceList list)
		{
			if (_unsubscribe != null)
				_unsubscribe();
			_unsubscribe = null;

			ITextBlock text = new TextBlock()
				.BeginBlock("Trace URI: " + _uriString,
					string.Format("{0} message{1}", list.Messages.Count, list.Messages.Count == 1 ? "" : "s"))
				.Break();

			foreach (ReceivedMessageTraceDetail message in list.Messages)
			{
				text.BeginBlock("Received: " + message.Id, Format(message.StartTime));

				text.Table(GetMessageHeaderDictionary(message),
					"Duration (ms)", ((int) message.Duration.TotalMilliseconds).ToString("N0"));

				text.Break();

				if (message.Receivers != null)
				{
					foreach (ReceiverTraceDetail receiver in message.Receivers)
					{
						text.BeginBlock(receiver.ReceiverType, Format(receiver.StartTime));
						text.Table(GetReceiverDictionary(receiver));
						text.EndBlock();
					}
				}

				if (message.SentMessages != null)
				{
					foreach (SentMessageTraceDetail sent in message.SentMessages)
					{
						text.BeginBlock("Sent: " + sent.Id, Format(sent.StartTime));

						text.Table(GetMessageHeaderDictionary(sent),
							"Endpoint Address", sent.Address.ToString());
						text.EndBlock();
					}
				}

				text.EndBlock();
			}

			_log.Info(text.ToString());
			_complete.Set();
		}

		public string Description
		{
			get { return string.Format("trace on {0}", _uriString); }
		}

		public WaitHandle WaitHandle
		{
			get { return _complete; }
		}

		string Format(DateTime value)
		{
			return value.ToString("yyyy-MM-dd hh:mm:ss.fff");
		}


		IDictionary<string, string> GetReceiverDictionary(ReceiverTraceDetail detail)
		{
			return GetReceiverValues(detail).ToDictionary(x => x.Key, x => x.Value);
		}

		IEnumerable<KeyValuePair<string, string>> GetReceiverValues(ReceiverTraceDetail detail)
		{
			if (detail.MessageType.IsNotEmpty())
				yield return new KeyValuePair<string, string>("Message Type", detail.MessageType);

			if (detail.CorrelationId.IsNotEmpty())
				yield return new KeyValuePair<string, string>("Correlation Id", detail.CorrelationId);

			if (detail.Duration != TimeSpan.Zero)
				yield return
					new KeyValuePair<string, string>("Duration (ms)", ((int) detail.Duration.TotalMilliseconds).ToString("N0"));
		}

		IDictionary<string, string> GetMessageHeaderDictionary(MessageTraceDetail detail)
		{
			return GetMessageHeaderValues(detail).ToDictionary(x => x.Key, x => x.Value);
		}

		IEnumerable<KeyValuePair<string, string>> GetMessageHeaderValues(MessageTraceDetail detail)
		{
			if (detail.MessageId.IsNotEmpty())
				yield return new KeyValuePair<string, string>("Message Id", detail.MessageId);

			if (detail.SourceAddress != null)
				yield return new KeyValuePair<string, string>("Source Address", detail.SourceAddress.ToString());

			if (detail.InputAddress != null)
				yield return new KeyValuePair<string, string>("Input Address", detail.InputAddress.ToString());

			if (detail.DestinationAddress != null)
				yield return new KeyValuePair<string, string>("Destination Address", detail.DestinationAddress.ToString());

			if (detail.ResponseAddress != null)
				yield return new KeyValuePair<string, string>("Response Address", detail.ResponseAddress.ToString());

			if (detail.FaultAddress != null)
				yield return new KeyValuePair<string, string>("Fault Address", detail.FaultAddress.ToString());

			if (detail.Network.IsNotEmpty())
				yield return new KeyValuePair<string, string>("Network", detail.Network);

			if (detail.ContentType.IsNotEmpty())
				yield return new KeyValuePair<string, string>("Content Type", detail.ContentType);

			if (detail.MessageType.IsNotEmpty())
				yield return new KeyValuePair<string, string>("Message Type", detail.MessageType);

			if (detail.ExpirationTime.HasValue)
				yield return
					new KeyValuePair<string, string>("Expiration Time",
						detail.ExpirationTime.Value.ToString("yyyy-MM-dd hh:mm:ss.ffff"));

			if (detail.RetryCount != 0)
				yield return new KeyValuePair<string, string>("Retry Count", detail.RetryCount.ToString());
		}
	}
}