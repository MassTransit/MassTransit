// Copyright 2007-2008 The Apache Software Foundation.
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
namespace BusDriver
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Commands;
	using log4net;
	using log4net.Appender;
	using log4net.Config;
	using log4net.Layout;
	using Magnum.CommandLineParser;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));
		private static readonly MonadicCommandLineParser _parser = new MonadicCommandLineParser();
		private static ConsoleAppender _appender;

		private static void Main(string[] args)
		{
			try
			{
				BootstrapLogger();

				_log.Info("Starting up ");

				if (args.Length > 1)
				{
					string line = GetUnparsedCommandLine();

					ProcessLine(line);
				}
				else
				{
					RunInteractiveConsole();
				}
			}
			finally
			{
				Console.WriteLine("Shutting down...");

				TransportCache.Clear();
			}
		}

		private static string GetUnparsedCommandLine()
		{
			string line = Environment.CommandLine;

			string applicationPath = Environment.GetCommandLineArgs()[0];

			string quotedApplicationPath = "\"" + applicationPath + "\" ";

			if (line.Substring(0, applicationPath.Length) == applicationPath)
				line = line.Substring(applicationPath.Length + 1);
			else if (line.Substring(0, quotedApplicationPath.Length) == quotedApplicationPath)
				line = line.Substring(quotedApplicationPath.Length);
			return line;
		}

		private static void RunInteractiveConsole()
		{
            _log.Info("Starting interactive console.");

			bool keepGoing = true;
			do
			{
				try
				{
					Console.Write("$ ");

					string line = Console.ReadLine();
					if(line.Trim().Length == 0)
						continue;

					keepGoing = ProcessLine(line);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			} while (keepGoing);
		}

		private static bool ProcessLine(string line)
		{
			IEnumerable<ICommandLineElement> elements = _parser.Parse(line);

			_log.Debug("Parsed: " + string.Join(", ", elements.Select(x => x.ToString()).ToArray()));

			ICommandLineElement element = elements.First();

			if (element is IArgumentElement)
			{
				var argument = element as IArgumentElement;

				if (argument.Id == "quit" || argument.Id == "exit")
					return false;

				if (argument.Id == "create")
					new CreateCommand(elements.Skip(1));
				
				if (argument.Id == "count")
					new CountEndpointCommand(elements.Skip(1));
				
				if (argument.Id == "send")
					new SendTextCommand(elements.Skip(1));
				
				if (argument.Id == "receive")
					new ReceiveCommand(elements.Skip(1));
				
				if (argument.Id == "move")
					new MoveCommand(elements.Skip(1));

                if (argument.Id == "moven")
                    new MoveNCommand(elements.Skip(1));
            }

			return true;
		}

		private static void BootstrapLogger()
		{
			_appender = new ConsoleAppender();
			_appender.Layout = new SimpleLayout();

			BasicConfigurator.Configure(_appender);
		}
	}
}