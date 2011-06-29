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
	using log4net;
	using MassTransit;
	using MassTransit.Diagnostics;

	public class TraceCommand :
		Command
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (TraceCommand));

		readonly int _count;
		readonly string _uriString;

		public TraceCommand(string uriString, int count)
		{
			_uriString = uriString;
			_count = count;
		}

		public bool Execute()
		{
			Uri uri = _uriString.ToUri("The trace URI was invalid");

			IServiceBus bus = Program.Bus;

			var endpoint = bus.GetEndpoint(uri);

			_log.InfoFormat("Sending trace request to {0} for {1} message{2}", uri, _count, _count == 1 ? "" : "s");
			var client = new MessageTraceClient(bus, endpoint, _count, x =>
				{
					string result = x.ToConsoleString();

					_log.Info("");
					_log.InfoFormat("Trace Response from {0} contained {1} message{2}", uri, x.Messages.Count, x.Messages.Count == 1 ? "" : "s");
					_log.Info(result);
					_log.Info("");
				});

			return true;
		}
	}
}