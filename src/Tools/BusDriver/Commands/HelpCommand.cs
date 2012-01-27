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
	using System.IO;
	using System.Reflection;
	using MassTransit.Logging;

    public class HelpCommand :
		Command
	{
		static readonly ILog _log = Logger.Get(typeof (HelpCommand));

		public bool Execute()
		{
			const string helpText = "BusDriver.HelpText.txt";

			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(helpText);
			if (stream == null)
			{
				_log.Error("Unable to load help text");
				return true;
			}

			using (TextReader reader = new StreamReader(stream))
			{
				string text = reader.ReadToEnd();
				_log.Info(text);
			}

			return true;
		}
	}
}