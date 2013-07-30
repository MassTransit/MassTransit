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

	public class MoveCommand :
		Command
	{
		static readonly ILog _log = Logger.Get(typeof (MoveCommand));
		readonly int _count;
		readonly string _fromUriString;
		readonly string _toUriString;

		public MoveCommand(string fromUriString, string toUriString, int count)
		{
			_fromUriString = fromUriString;
			_toUriString = toUriString;
			_count = count;
		}

		public bool Execute()
		{
			Uri fromUri = _fromUriString.ToUri("The from URI was invalid");
			Uri toUri = _toUriString.ToUri("The to URI was invalid");

			IInboundTransport fromTransport = Program.Transports.GetInboundTransport(fromUri);
			IOutboundTransport toTransport = Program.Transports.GetOutboundTransport(toUri);

			ITextBlock text = new TextBlock()
				.BeginBlock("Move messages from " + fromUri + " to " + toUri, "");

			int moveCount = 0;
			for (int i = 0; i < _count; i++)
			{
				fromTransport.Receive(receiveContext =>
					{
						return context =>
							{
								var moveContext = new MoveMessageSendContext(context);

								toTransport.Send(moveContext);

								text.BodyFormat("Message-Id: {0}", context.MessageId);

								moveCount++;
							};
                    }, 5.Seconds());
			}

			_log.Info(text);
			_log.InfoFormat("{0} message{1} moved from {2} to {3}", moveCount, moveCount == 1 ? "" : "s", fromUri, toUri);

			return true;
		}
	}
}