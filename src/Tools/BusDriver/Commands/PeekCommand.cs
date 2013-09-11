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
	using System.Text;
	using Formatting;
	using MassTransit.Logging;
	using Magnum.Extensions;
	using MassTransit.Transports;

	public class PeekCommand :
		Command
	{
		static readonly ILog _log = Logger.Get(typeof(PeekCommand));
		readonly int _count;
		readonly string _uriString;

		public PeekCommand(string uriString, int count)
		{
			_uriString = uriString;
			_count = count;
		}

		public bool Execute()
		{
			Uri uri = _uriString.ToUri("The from URI was invalid");

			IInboundTransport fromTransport = Program.Transports.GetInboundTransport(uri);

			var text = new TextBlock()
				.BeginBlock("Peek URI: " + uri, "");

			int peekCount = 0;
			fromTransport.Receive(receiveContext =>
				{
					if (peekCount >= _count)
						return null;

					var body = Encoding.UTF8.GetString(receiveContext.BodyStream.ReadToEnd());

					text.BeginBlock("MessageId: " + receiveContext.MessageId ?? "", peekCount)
						.Body(body)
						.EndBlock();

					peekCount++;

					return null;
				}, 5.Seconds());

			_log.Info(text.ToString());

			return true;
		}
	}
}