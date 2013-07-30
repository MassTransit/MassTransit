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
	using Formatting;
	using Magnum.Extensions;
	using MassTransit.Logging;
	using MassTransit.Context;
	using MassTransit.Transports;

	public class RequeueCommand :
		Command
	{
		static readonly ILog _log = Logger.Get(typeof(RequeueCommand));
		readonly int _count;
		readonly string _uriString;

		public RequeueCommand(string uriString, int count)
		{
			_uriString = uriString;
			_count = count;
		}

		public bool Execute()
		{
			Uri uri = _uriString.ToUri("The URI was invalid");

			IInboundTransport inboundTransport = Program.Transports.GetInboundTransport(uri);
			IOutboundTransport outboundTransport = Program.Transports.GetOutboundTransport(uri);

			ITextBlock text = new TextBlock()
				.BeginBlock("Requeue messages to " + uri, "");


			int requeueCount = 0;
			for (int i = 0; i < _count; i++)
			{
				inboundTransport.Receive(receiveContext =>
					{
						return context =>
							{
								var moveContext = new MoveMessageSendContext(context);

                                outboundTransport.Send(moveContext);

								text.BodyFormat("Message-Id: {0}", context.MessageId);

								requeueCount++;
							};
					}, 5.Seconds());
			}

			_log.Info(text);
			_log.InfoFormat("{0} message{1} requeued to {2}", requeueCount, requeueCount == 1 ? "" : "s", uri);

			return true;
		}
	}
}