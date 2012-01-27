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
	using MassTransit.Logging;
	using MassTransit.Transports.Msmq;

	public class CountCommand :
		Command
	{
		static readonly ILog _log = Logger.Get(typeof (CountCommand));
		readonly string _uriString;

		public CountCommand(string uriString)
		{
			_uriString = uriString;
		}

		public bool Execute()
		{
			Uri uri = _uriString.ToUri("The endpoint URI was invalid");

			if (string.Compare(uri.Scheme, "msmq", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				IEndpointManagement management = MsmqEndpointManagement.New(uri);

				long count = management.Count();

				_log.Info(new TextBlock()
					.BeginBlock("Count URI: " + uri, "")
					.BodyFormat("{0} message{1}", count, count != 1 ? "s" : "")
					.EndBlock()
					.ToString());

				return true;
			}

			_log.ErrorFormat("Count is not supported for the specified transport type: {0}", uri.Scheme);

			return true;
		}
	}
}